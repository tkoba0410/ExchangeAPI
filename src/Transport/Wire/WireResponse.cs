using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Transport.Wire;

public sealed record WireResponse(
    ExchangeCode Exchange,
    int StatusCode,
    string Json,
    IReadOnlyDictionary<string, string>? Headers = null,
    string? RequestId = null,
    long? ElapsedMs = null);
