using System.Collections.Generic;

namespace ExchangeApi.Transport.Wire;

public sealed record WireResponse(
    int StatusCode,
    string Json,
    IReadOnlyDictionary<string, string>? Headers = null,
    string? RequestId = null,
    long? ElapsedMs = null);
