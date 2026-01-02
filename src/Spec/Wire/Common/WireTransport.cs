using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Spec.Wire;

/// <summary>
/// Wire 層の送信実装。HTTP ステータス(4xx/5xx)は例外化せず Raw 層で判定する。
/// 例外は transport レベル（接続失敗、タイムアウト、TLS、キャンセル等）のみ。
/// </summary>
public sealed class WireTransport : IWireTransport
{
    private readonly IRestClient _restClient;

    public WireTransport(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    /// <summary>
    /// WireCall を生成して返す。RequestId が取得できる場合は Meta に設定する。
    /// </summary>
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
