using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;
using Common.Transport.Protocol;

namespace ExchangeApi.Adapter.Bittrade;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw 実装。
/// </summary>
public sealed class BittradePrivateTradingApi : IBittradePrivateTradingApi
{
    private readonly IRestClient _restClient;

    public BittradePrivateTradingApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<BittradePlaceOrderResponse> PlaceOrderAsync(
        Dictionary<string, object?> body,
        CancellationToken cancellationToken = default)
    {
        if (body is null) throw new ArgumentNullException(nameof(body));

        return _restClient.PostAsync<Dictionary<string, object?>, BittradePlaceOrderResponse>(
            "v1/order/orders/place",
            body,
            cancellationToken);
    }

    public Task<BittradeCancelOrderResponse> CancelOrderAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        return _restClient.PostAsync<object?, BittradeCancelOrderResponse>(
            $"v1/order/orders/{orderId}/submitcancel",
            body: null,
            cancellationToken: cancellationToken);
    }
}
