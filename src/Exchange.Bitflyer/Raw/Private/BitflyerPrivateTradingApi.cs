using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer Private Trading REST API の実装（発注・キャンセル系）。
/// </summary>
public sealed class BitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly IRestClient _restClient;

    public BitflyerPrivateTradingApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(
        BitflyerSendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = "/v1/me/sendchildorder";

        return _restClient.PostAsync<BitflyerSendChildOrderRequest, BitflyerSendChildOrderResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<BitflyerEmptyResponse> CancelChildOrderAsync(
        BitflyerCancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = "/v1/me/cancelchildorder";

        return _restClient.PostAsync<BitflyerCancelChildOrderRequest, BitflyerEmptyResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(
        BitflyerCancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = "/v1/me/cancelallchildorders";

        return _restClient.PostAsync<BitflyerCancelAllChildOrdersRequest, BitflyerEmptyResponse>(
            path,
            request,
            cancellationToken);
    }
}
