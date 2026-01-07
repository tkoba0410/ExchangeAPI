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
    /// Call を生成して返す。RequestId が取得できる場合は Meta に設定する。
    /// </summary>
    public async Task<Call<WireCallSpec, WireResponse>> SendAsync(
        ExchangeCode exchange,
        WireCallSpec request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var startedAt = DateTimeOffset.UtcNow;
        try
        {
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
            var meta = new CallMeta(
                Layer: "Wire",
                Component: $"{exchange}.SendRawAsync",
                Tags: null,
                Children: null);

            return new Call<WireCallSpec, WireResponse>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: elapsed,
                Request: request,
                Result: new CallResult<WireResponse>.Ok(response),
                Meta: meta);
        }
        catch (OperationCanceledException)
        {
            var elapsed = DateTimeOffset.UtcNow - startedAt;
            var error = new CallError(CallErrorKind.Transport, "Wire transport canceled.");
            var meta = new CallMeta(
                Layer: "Wire",
                Component: $"{exchange}.SendRawAsync",
                Tags: null,
                Children: null);
            return new Call<WireCallSpec, WireResponse>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: elapsed,
                Request: request,
                Result: new CallResult<WireResponse>.Err(error),
                Meta: meta);
        }
        catch (Exception ex)
        {
            var elapsed = DateTimeOffset.UtcNow - startedAt;
            var error = new CallError(CallErrorKind.Transport, "Wire transport failed.", ex);
            var meta = new CallMeta(
                Layer: "Wire",
                Component: $"{exchange}.SendRawAsync",
                Tags: null,
                Children: null);
            return new Call<WireCallSpec, WireResponse>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: elapsed,
                Request: request,
                Result: new CallResult<WireResponse>.Err(error),
                Meta: meta);
        }
    }
}
