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

    public Task<PlaceOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<CreateOrderRequest, PlaceOrderResponse>(
            "v1/order/orders/place",
            request,
            cancellationToken);
    }

    public Task<CancelOrderResponse> CancelOrderAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        return _restClient.PostAsync<object?, CancelOrderResponse>(
            $"v1/order/orders/{orderId}/submitcancel",
            body: null,
            cancellationToken: cancellationToken);
    }

    public Task<CancelOrdersResponse> CancelOrdersAsync(CancelOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<CancelOrdersRequest, CancelOrdersResponse>(
            "v1/order/orders/batchcancel",
            request,
            cancellationToken);
    }

    public Task<CancelOpenOrdersResponse> CancelOpenOrdersAsync(CancelOpenOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<CancelOpenOrdersRequest, CancelOpenOrdersResponse>(
            "v1/order/orders/batchCancelOpenOrders",
            request,
            cancellationToken);
    }

    public Task<CreateWithdrawResponse> CreateWithdrawAsync(CreateWithdrawRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<CreateWithdrawRequest, CreateWithdrawResponse>(
            "v1/dw/withdraw/api/create",
            request,
            cancellationToken);
    }

    public Task<CancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(withdrawId))
        {
            throw new ArgumentException("withdrawId is required.", nameof(withdrawId));
        }

        return _restClient.PostAsync<object?, CancelWithdrawResponse>(
            $"v1/dw/withdraw-virtual/{withdrawId}/cancel",
            body: null,
            cancellationToken: cancellationToken);
    }

    public Task<RetailOrderResponse> CreateRetailOrderAsync(CreateRetailOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        return _restClient.PostAsync<CreateRetailOrderRequest, RetailOrderResponse>(
            "v1/retail/order/place",
            request,
            cancellationToken);
    }
}
