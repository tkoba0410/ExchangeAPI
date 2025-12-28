using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw 実装。
/// </summary>
internal sealed class BittradePrivateTradingApi : IBittradePrivateTradingApi
{
    private readonly IRestClient _restClient;

    public BittradePrivateTradingApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<RawCreateOrderRequest, RawPlaceOrderResponse>(
            "v1/order/orders/place",
            request,
            cancellationToken);
    }

    public Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId.Value))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        return _restClient.PostAsync<object?, RawCancelOrderResponse>(
            $"v1/order/orders/{orderId.Value}/submitcancel",
            body: null,
            cancellationToken: cancellationToken);
    }

    public Task<RawCancelOrdersResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<RawCancelOrdersRequest, RawCancelOrdersResponse>(
            "v1/order/orders/batchcancel",
            request,
            cancellationToken);
    }

    public Task<RawCancelOpenOrdersResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<RawCancelOpenOrdersRequest, RawCancelOpenOrdersResponse>(
            "v1/order/orders/batchCancelOpenOrders",
            request,
            cancellationToken);
    }

    public Task<RawCreateWithdrawResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<RawCreateWithdrawRequest, RawCreateWithdrawResponse>(
            "v1/dw/withdraw/api/create",
            request,
            cancellationToken);
    }

    public Task<RawCancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(withdrawId))
        {
            throw new ArgumentException("withdrawId is required.", nameof(withdrawId));
        }

        return _restClient.PostAsync<object?, RawCancelWithdrawResponse>(
            $"v1/dw/withdraw-virtual/{withdrawId}/cancel",
            body: null,
            cancellationToken: cancellationToken);
    }

    public Task<RawRetailOrderResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<RawCreateRetailOrderRequest, RawRetailOrderResponse>(
            "v1/retail/order/place",
            request,
            cancellationToken);
    }
}
