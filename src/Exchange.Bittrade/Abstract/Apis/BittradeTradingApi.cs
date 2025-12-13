using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.Adapters;
using ExchangeApi.Adapter.Bittrade.RawApi;
using Common.Contract.Interfaces;
using Common.Contract.Dtos;
using Common.Contract.Errors;
using Common.Transport.Protocol;

namespace ExchangeApi.Adapter.Bittrade.Apis;

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
        var resp = await _restClient.GetAsync<BittradeBalancesResponse>(
            $"v1/account/accounts/{_accountId}/balance",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
        {
            throw new ExchangeApiException("Bittrade balance response invalid.");
        }

        return BittradeMapper.MapBalances(resp.Data);
    }

    public async Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var apiSymbol = ToApiSymbol(request.ProductCode);
        var type = ToOrderType(request);

        var body = new Dictionary<string, object?>
        {
            ["account-id"] = _accountId,
            ["symbol"] = apiSymbol,
            ["type"] = type,
            ["amount"] = request.Size.ToString()
        };

        if (request.OrderType == OrderType.Limit)
        {
            body["price"] = request.Price?.ToString();
        }

        if (!string.IsNullOrWhiteSpace(request.ClientOrderId))
        {
            body["client-order-id"] = request.ClientOrderId;
        }

        var resp = await _restClient.PostAsync<Dictionary<string, object?>, BittradePlaceOrderResponse>(
            "v1/order/orders/place",
            body,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExchangeApiException("Bittrade place order failed.");
        }

        return new OrderResult(resp.OrderId.ToString());
    }

    public async Task<CancelResult> CancelOrderAsync(string productCode, string orderId, CancellationToken cancellationToken = default)
    {
        var resp = await _restClient.PostAsync<object?, BittradeCancelOrderResponse>(
            $"v1/order/orders/{orderId}/submitcancel",
            body: null,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExchangeApiException("Bittrade cancel order failed.");
        }

        return new CancelResult(true);
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(string productCode, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(productCode);
        var resp = await _restClient.GetAsync<BittradeOpenOrdersResponse>(
            $"v1/order/openOrders?symbol={apiSymbol}&account-id={_accountId}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
        {
            throw new ExchangeApiException("Bittrade open orders response invalid.");
        }

        return resp.Data.Select(BittradeMapper.MapOrderSummary).ToList();
    }

    public Task<OrderStatus> PollOrderStatusAsync(
        string productCode,
        string orderId,
        TimeSpan? pollInterval = null,
        int maxAttempts = 30,
        CancellationToken cancellationToken = default)
    {
        // Bittrade は即時に詳細を返すため、単一呼び出しのみ実施（pollInterval/maxAttempts は無視）。
        return PollOrderStatusOnceAsync(orderId, cancellationToken);
    }

    private async Task<OrderStatus> PollOrderStatusOnceAsync(string orderId, CancellationToken cancellationToken)
    {
        var resp = await _restClient.GetAsync<BittradeOrderDetailResponse>(
            $"v1/order/orders/{orderId}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
        {
            throw new ExchangeApiException("Bittrade order detail response invalid.");
        }

        var order = BittradeMapper.MapOrder(resp.Data);
        return new OrderStatus(
            order.ProductCode,
            orderId,
            BittradeMapper.ParseStatus(resp.Data.State),
            order.ExecutedSize,
            order.OutstandingSize,
            order.Price,
            null);
    }

    public Task<IReadOnlyList<AccountExecution>> GetAccountExecutionsAsync(string productCode, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Bittrade account executions are not provided via REST in this adapter.");
    }

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static string ToOrderType(OrderRequest request)
    {
        return (request.Side, request.OrderType) switch
        {
            (OrderSide.Buy, OrderType.Market) => "buy-market",
            (OrderSide.Sell, OrderType.Market) => "sell-market",
            (OrderSide.Buy, OrderType.Limit) => "buy-limit",
            (OrderSide.Sell, OrderType.Limit) => "sell-limit",
            _ => throw new ExchangeApiException($"Unsupported order type: {request.OrderType}")
        };
    }
}
