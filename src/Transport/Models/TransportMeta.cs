using System;

namespace ExchangeApi.Transport.Models;

/// <summary>通信メタ情報（リクエスト/レスポンス、時刻など）。</summary>
public sealed record TransportMeta(
    DateTimeOffset LocalSend,
    DateTimeOffset LocalReceive,
    DateTimeOffset? ServerTimestamp,
    HttpRequestMeta Request,
    HttpResponseMeta Response);
