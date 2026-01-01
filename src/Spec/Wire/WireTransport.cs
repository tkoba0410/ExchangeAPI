using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Spec.Wire;

public sealed class WireTransport : IWireTransport
{
    private readonly IRestClient _restClient;

    public WireTransport(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<WireCall> SendAsync(
        ExchangeCode exchange,
        WireRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var startedAt = DateTimeOffset.UtcNow;
        var responseMeta = await _restClient
            .SendRawAsync(request.Method, request.Path, request.Query, request.BodyJson, request.Headers, ct)
            .ConfigureAwait(false);
        var elapsed = DateTimeOffset.UtcNow - startedAt;

        var headers = responseMeta.Headers is null
            ? null
            : new Dictionary<string, string>(responseMeta.Headers, StringComparer.OrdinalIgnoreCase);
        var response = new WireResponse(
            Exchange: exchange,
            StatusCode: responseMeta.StatusCode,
            Json: responseMeta.Body ?? string.Empty,
            Headers: headers,
            RequestId: null,
            ElapsedMs: (long)elapsed.TotalMilliseconds);
        var meta = new CallMeta(startedAt, elapsed, response.RequestId);

        return new WireCall(request, response, meta);
    }
}
