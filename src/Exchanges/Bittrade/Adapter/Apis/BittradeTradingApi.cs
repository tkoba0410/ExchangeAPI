using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Enums;
using CommonOrderType = ExchangeApi.Common.Enums.OrderType;
using RawOrderType = ExchangeApi.Exchanges.Bittrade.Raw.OrderType;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using RawSymbol = ExchangeApi.Exchanges.Bittrade.Raw.Symbol;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class BittradeTradingApi : ITradingApi
{
    private readonly IRestClient _restClient;
    private readonly string _accountId;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeTradingApi(IRestClient restClient, string accountId)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public Task<OrderResult> PlaceLimitOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Limit(symbol, side, size, price), "Bittrade.Trading.PlaceLimitOrder", cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Market(symbol, side, size), "Bittrade.Trading.PlaceMarketOrder", cancellationToken);

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
            var apiSymbol = ToApiSymbol(request.Symbol);
            var type = ToOrderType(request);

            var dto = new CreateOrderRequest(
                AccountId: _accountId,
                Symbol: new RawSymbol(apiSymbol),
                Type: type,
                Amount: request.Size.ToString(),
                Price: request.OrderType == CommonOrderType.Limit ? request.Price?.ToString() : null,
                Source: null);

            var resp = await _restClient.PostAsync<CreateOrderRequest, PlaceOrderResponse>(
                "v1/order/orders/place",
                dto,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase))
            {
                throw new ExchangeApiException(
                    message: "Bittrade place order failed.",
                    exchange: Exchange,
                    operation: operation);
            }

            var orderId = resp.OrderId.Value;
            var key = new OrderKey(OrderIdKind.ExchangeOrderId, orderId);
            return new OrderResult(key, ExchangeOrderId: orderId);
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

        const string operation = "Bittrade.Trading.CancelOrder";
        try
        {
            var resp = await _restClient.PostAsync<object?, CancelOrderResponse>(
                $"v1/order/orders/{orderKey.Value}/submitcancel",
                body: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase))
            {
                throw new ExchangeApiException(
                    message: "Bittrade cancel order failed.",
                    exchange: Exchange,
                    operation: operation);
            }

            return new CancelResult(true);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Trading.GetOpenOrders";
        try
        {
            var apiSymbol = ToApiSymbol(symbol);
            var resp = await _restClient.GetAsync<OpenOrdersResponse>(
                $"v1/order/openOrders?symbol={apiSymbol}&account-id={_accountId}",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
            {
                throw new ExchangeApiException(
                    message: "Bittrade open orders response invalid.",
                    exchange: Exchange,
                    operation: operation);
            }

            return resp.Data.Select(BittradeMapper.MapOrderSummary).ToList();
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

        const string operation = "Bittrade.Trading.GetOrder";
        try
        {
            var resp = await _restClient.GetAsync<OrderDetailResponse>(
                $"v1/order/orders/{orderKey.Value}",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
            {
                throw new ExchangeApiException(
                    message: "Bittrade order detail response invalid.",
                    exchange: Exchange,
                    operation: operation);
            }

            var order = BittradeMapper.MapOrder(resp.Data);
            var productCode = BittradeMapper.ToProductCode(order.Symbol);
            var key = orderKey.Kind == OrderIdKind.AcceptanceId
                ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
                : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);
            return new OrderStatus(
                productCode,
                key,
                BittradeMapper.ParseStatus(resp.Data.State),
                order.ExecutedSize,
                order.OutstandingSize,
                order.Price,
                null);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    private static string ToApiSymbol(CommonSymbol symbol) =>
        BittradeSymbolMapper.ToApiSymbol(symbol);

    private static RawOrderType ToOrderType(OrderRequest request) =>
        (request.Side, request.OrderType) switch
        {
            (Side.Buy, CommonOrderType.Market) => RawOrderType.BuyMarket,
            (Side.Sell, CommonOrderType.Market) => RawOrderType.SellMarket,
            (Side.Buy, CommonOrderType.Limit) => RawOrderType.BuyLimit,
            (Side.Sell, CommonOrderType.Limit) => RawOrderType.SellLimit,
            _ => throw new ExchangeApiException($"Unsupported order type: {request.OrderType}")
        };
}
