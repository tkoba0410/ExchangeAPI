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
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class BittradeTradingApi : ITradingApi
{
    private readonly IBittradeNormalizedTradingApi _trading;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeTradingApi(IBittradeNormalizedTradingApi trading)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
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
            return await _trading.PlaceOrderAsync(request, cancellationToken).ConfigureAwait(false);
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
            return await _trading.CancelOrderAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
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
            return await _trading.GetOpenOrdersAsync(symbol, cancellationToken).ConfigureAwait(false);
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
            return await _trading.GetOrderAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
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

}
