using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Enums;
using CommonOrderType = ExchangeApi.Common.Enums.OrderType;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
public sealed class BittradeTradingApi : ITradingApi, IAccountApi
{
    private readonly IRestClient _restClient;
    private readonly string _accountId;

    public BittradeTradingApi(IRestClient restClient, string accountId)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        var resp = await _restClient.GetAsync<BalancesResponse>(
            $"v1/account/accounts/{_accountId}/balance",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
        {
            throw new ExchangeApiException("Bittrade balance response invalid.");
        }

        return BittradeMapper.MapBalances(resp.Data);
    }

    public Task<OrderResult> PlaceLimitOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Limit(symbol, side, size, price), cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Market(symbol, side, size), cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "StopOrder");

    private async Task<OrderResult> PlaceOrderInternal(OrderRequest request, CancellationToken cancellationToken)
    {
        var apiSymbol = ToApiSymbol(request.Symbol);
        var type = ToOrderType(request);

        var body = new Dictionary<string, object?>
        {
            ["account-id"] = _accountId,
            ["symbol"] = apiSymbol,
            ["type"] = type,
            ["amount"] = request.Size.ToString()
        };

        if (request.OrderType == CommonOrderType.Limit)
        {
            body["price"] = request.Price?.ToString();
        }

        var resp = await _restClient.PostAsync<Dictionary<string, object?>, PlaceOrderResponse>(
            "v1/order/orders/place",
            body,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExchangeApiException("Bittrade place order failed.");
        }

        var orderId = resp.OrderId.Value;
        var key = new OrderKey(OrderIdKind.ExchangeOrderId, orderId);
        return new OrderResult(key, ExchangeOrderId: orderId);
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

        var resp = await _restClient.PostAsync<object?, CancelOrderResponse>(
            $"v1/order/orders/{orderKey.Value}/submitcancel",
            body: null,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExchangeApiException("Bittrade cancel order failed.");
        }

        return new CancelResult(true);
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(symbol);
        var resp = await _restClient.GetAsync<OpenOrdersResponse>(
            $"v1/order/openOrders?symbol={apiSymbol}&account-id={_accountId}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
        {
            throw new ExchangeApiException("Bittrade open orders response invalid.");
        }

        return resp.Data.Select(BittradeMapper.MapOrderSummary).ToList();
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

        var resp = await _restClient.GetAsync<OrderDetailResponse>(
            $"v1/order/orders/{orderKey.Value}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
        {
            throw new ExchangeApiException("Bittrade order detail response invalid.");
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

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "AccountExecutions");
    }

    private static string ToApiSymbol(CommonSymbol symbol) =>
        BittradeSymbolMapper.ToApiSymbol(symbol);

    private static string ToOrderType(OrderRequest request) =>
        (request.Side, request.OrderType) switch
        {
            (Side.Buy, CommonOrderType.Market) => "buy-market",
            (Side.Sell, CommonOrderType.Market) => "sell-market",
            (Side.Buy, CommonOrderType.Limit) => "buy-limit",
            (Side.Sell, CommonOrderType.Limit) => "sell-limit",
            _ => throw new ExchangeApiException($"Unsupported order type: {request.OrderType}")
        };
}
