using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bittrade.RawApiClient;

/// <summary>
/// Bittrade の Raw API ラッパー（最低限の Public/Private）。
/// </summary>
public sealed class BittradeRawApiClient
{
    private readonly IRestClient _restClient;

    public BittradeRawApiClient(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<BittradeMergedResponse> GetTickerAsync(string symbol, CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<BittradeMergedResponse>($"market/detail/merged?symbol={ToApiSymbol(symbol)}", cancellationToken: cancellationToken);

    public Task<BittradeDepthResponse> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<BittradeDepthResponse>($"market/depth?symbol={ToApiSymbol(symbol)}&type=step0", cancellationToken: cancellationToken);

    public Task<BittradeTradeResponse> GetTradesAsync(string symbol, CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<BittradeTradeResponse>($"market/trade?symbol={ToApiSymbol(symbol)}", cancellationToken: cancellationToken);

    public Task<BittradeBalancesResponse> GetBalancesAsync(string accountId, CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<BittradeBalancesResponse>($"v1/account/accounts/{accountId}/balance", cancellationToken: cancellationToken);

    public Task<BittradePlaceOrderResponse> PlaceOrderAsync(Dictionary<string, object?> body, CancellationToken cancellationToken = default) =>
        _restClient.PostAsync<Dictionary<string, object?>, BittradePlaceOrderResponse>("v1/order/orders/place", body, cancellationToken);

    public Task<BittradeCancelOrderResponse> CancelOrderAsync(string orderId, CancellationToken cancellationToken = default) =>
        _restClient.PostAsync<object?, BittradeCancelOrderResponse>($"v1/order/orders/{orderId}/submitcancel", null, cancellationToken);

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
}
