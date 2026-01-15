using System.Collections.Generic;

namespace ExchangeApi.Transport.Wire;

public sealed record WireCallSpec(
    string Method,
    string Path,
    string? Query = null,
    string? BodyJson = null,
    IReadOnlyDictionary<string, string>? Headers = null);
