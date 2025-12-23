using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class BittradeTradingApi : ITradingApi
{
    private readonly IBittradeWireTradingApi _wire;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeTradingApi(IBittradeWireTradingApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
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
            var wireRequest = BittradeTradingMapper.ToWire(request);
            var wire = await _wire.PlaceOrderAsync(wireRequest, cancellationToken).ConfigureAwait(false);
            return BittradeTradingMapper.ToOrderResult(wire);
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
            await _wire.CancelOrderAsync(orderKey.Value, cancellationToken).ConfigureAwait(false);

            return new CancelResult(true);
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
            var wire = await _wire.GetOpenOrdersAsync(symbol.Value, cancellationToken).ConfigureAwait(false);
            return wire.Select(BittradeTradingMapper.ToOpenOrder).ToList();
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
            var key = orderKey.Kind == OrderIdKind.AcceptanceId
                ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
                : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

            var wire = await _wire.GetOrderAsync(orderKey.Value, cancellationToken).ConfigureAwait(false);
            return BittradeTradingMapper.ToOrderStatus(wire, key);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }
}
