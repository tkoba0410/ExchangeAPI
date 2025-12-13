using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw;
using ExchangeApi.Transport.Protocol;

namespace Exchange.Bitflyer.Raw;

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

        const string path = BitflyerConstants.Paths.SendChildOrder;

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

        const string path = BitflyerConstants.Paths.CancelChildOrder;

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

        const string path = BitflyerConstants.Paths.CancelAllChildOrders;

        return _restClient.PostAsync<BitflyerCancelAllChildOrdersRequest, BitflyerEmptyResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<JsonElement> SendParentOrderAsync(
        Dictionary<string, object?> body,
        CancellationToken cancellationToken = default)
    {
        if (body is null) throw new ArgumentNullException(nameof(body));
        const string path = BitflyerConstants.Paths.SendParentOrder;
        return _restClient.PostAsync<Dictionary<string, object?>, JsonElement>(path, body, cancellationToken);
    }

    public Task<BitflyerEmptyResponse> CancelParentOrderAsync(
        Dictionary<string, object?> body,
        CancellationToken cancellationToken = default)
    {
        if (body is null) throw new ArgumentNullException(nameof(body));
        const string path = BitflyerConstants.Paths.CancelParentOrder;
        return _restClient.PostAsync<Dictionary<string, object?>, BitflyerEmptyResponse>(path, body, cancellationToken);
    }

    public Task<JsonElement> WithdrawAsync(
        Dictionary<string, object?> body,
        CancellationToken cancellationToken = default)
    {
        if (body is null) throw new ArgumentNullException(nameof(body));
        const string path = BitflyerConstants.Paths.Withdraw;
        return _restClient.PostAsync<Dictionary<string, object?>, JsonElement>(path, body, cancellationToken);
    }
}
