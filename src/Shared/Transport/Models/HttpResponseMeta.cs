using System.Collections.Generic;
namespace ExchangeApi.Shared.Transport.Models;

/// <summary>HTTP レスポンスのメタ情報。</summary>
public sealed record HttpResponseMeta(
    int StatusCode,
    IDictionary<string, string>? Headers,
    string? Body);
