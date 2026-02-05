using System;
using System.Collections.Generic;
namespace ExchangeApi.Transport.Models;

/// <summary>HTTP リクエストのメタ情報（秘密情報は含めない）。</summary>
public sealed record HttpRequestMeta(
    string Method,
    Uri Endpoint,
    IDictionary<string, string>? Headers,
    string? Body);
