using System.Collections.Generic;
using ExchangeApi.Common.Enums;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

public sealed record WireResponse(
    ExchangeCode Exchange,
    int StatusCode,
    string Json,
    IReadOnlyDictionary<string, string>? Headers = null,
    string? RequestId = null,
    long? ElapsedMs = null);
