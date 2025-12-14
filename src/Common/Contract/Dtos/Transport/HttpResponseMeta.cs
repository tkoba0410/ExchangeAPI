using System.Collections.Generic;

namespace Common.Contract.Dtos.Transport;

/// <summary>HTTP レスポンスのメタ情報。</summary>
public sealed record HttpResponseMeta(
    int StatusCode,
    IDictionary<string, string>? Headers,
    string? Body);
