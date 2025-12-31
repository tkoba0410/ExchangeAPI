using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Raw;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class BittradeTradingApi : ITradingApi
{
    private readonly IBittradeRawTradingApi _trading;
    private readonly IExchangeMarketResolver _markets;
    private readonly string _accountId;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeTradingApi(IBittradeRawTradingApi trading, IExchangeMarketResolver markets, string accountId)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
        _accountId = string.IsNullOrWhiteSpace(accountId)
            ? throw new ArgumentException("accountId is required.", nameof(accountId))
            : accountId;
    }

    public Task<OrderResult> PlaceLimitOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Limit(symbol, side, size, price), BittradeOperations.Trading.PlaceOrder, cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Market(symbol, side, size), BittradeOperations.Trading.PlaceOrder, cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(Exchange, "StopOrder");

    private async Task<OrderResult> PlaceOrderInternal(OrderRequest request, string operation, CancellationToken cancellationToken)
    {
        try
        {
            var apiSymbol = await ToApiSymbolAsync(request.Symbol, cancellationToken).ConfigureAwait(false);
            var rawRequest = BittradeTradingMapper.ToRaw(_accountId, apiSymbol, request);
            var raw = await _trading.CreateOrderAsync(rawRequest, cancellationToken).ConfigureAwait(false);
            return BittradeTradingMapper.ToOrderResult(raw);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<CancelResult> CancelOrderAsync(CommonSymbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, $"CancelOrderBy{orderKey.Kind}");
        }

        const string operation = BittradeOperations.Trading.CancelOrder;
        try
        {
            await _trading.CancelOrderAsync(RawOrderId.From(orderKey.Value), cancellationToken).ConfigureAwait(false);

            return new CancelResult(true);
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        const string operation = BittradeOperations.Trading.GetOpenOrders;
        try
        {
            var apiSymbol = await ToApiSymbolAsync(symbol, cancellationToken).ConfigureAwait(false);
            var raw = await _trading.GetOpenOrdersAsync(RawSymbol.From(apiSymbol), _accountId, cancellationToken).ConfigureAwait(false);
            return BittradeTradingMapper.ToOpenOrders(symbol, raw);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<OrderStatus> GetOrderAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, $"GetOrderBy{orderKey.Kind}");
        }

        const string operation = BittradeOperations.Trading.GetOrder;
        try
        {
            var market = await _markets.ResolveAsync(symbol, cancellationToken).ConfigureAwait(false);
            var key = orderKey.Kind == OrderIdKind.AcceptanceId
                ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
                : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

            var raw = await _trading.GetOrderAsync(RawOrderId.From(orderKey.Value), cancellationToken).ConfigureAwait(false);
            return BittradeTradingMapper.ToOrderStatus(market.ProductCode, raw, key);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    private async Task<string> ToApiSymbolAsync(CommonSymbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }
}
