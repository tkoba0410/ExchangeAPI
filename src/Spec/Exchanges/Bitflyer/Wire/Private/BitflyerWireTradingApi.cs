using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireTradingApi : IBitflyerWireTradingApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;
    private readonly IRestClient _restClient;

    public BitflyerWireTradingApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<WireCall> CreateChildOrderAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        PostAsync(BitflyerConstants.Paths.SendChildOrder, request, cancellationToken);

    public Task<WireCall> CancelChildOrderAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        PostAsync(BitflyerConstants.Paths.CancelChildOrder, request, cancellationToken);

    public Task<WireCall> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        PostAsync(BitflyerConstants.Paths.CancelAllChildOrders, request, cancellationToken);

    public Task<WireCall> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        PostAsync(BitflyerConstants.Paths.SendParentOrder, request, cancellationToken);

    public Task<WireCall> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        PostAsync(BitflyerConstants.Paths.CancelParentOrder, request, cancellationToken);

    public Task<WireCall> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        PostAsync(BitflyerConstants.Paths.Withdraw, request, cancellationToken);

    private async Task<WireCall> PostAsync<TRequest>(
        string path,
        TRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var requestJson = JsonSerializer.Serialize(request);
        var wireRequest = new WireRequest(
            Method: "POST",
            Path: path,
            Query: null,
            BodyJson: requestJson);
        var meta = await _restClient.PostRawAsync(path, request, cancellationToken).ConfigureAwait(false);
        var response = ToWire(meta);
        return new WireCall(wireRequest, response, CreateMeta(response));
    }

    private static WireResponse ToWire(HttpResponseMeta meta)
    {
        var headers = meta.Headers is null
            ? null
            : new Dictionary<string, string>(meta.Headers, StringComparer.OrdinalIgnoreCase);
        return new WireResponse(
            Exchange,
            meta.StatusCode,
            meta.Body ?? string.Empty,
            headers);
    }

    private static CallMeta CreateMeta(WireResponse response)
    {
        var elapsed = response.ElapsedMs is { } ms ? TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero;
        var startedAt = DateTimeOffset.UtcNow - elapsed;
        return new CallMeta(startedAt, elapsed, response.RequestId);
    }
}
