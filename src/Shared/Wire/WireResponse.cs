using System.Collections.Generic;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;

namespace ExchangeApi.Shared.Wire;

public sealed record WireResponse(
    ExchangeCode Exchange,
    int StatusCode,
    string Json,
    IReadOnlyDictionary<string, string>? Headers = null,
    string? RequestId = null,
    long? ElapsedMs = null);
