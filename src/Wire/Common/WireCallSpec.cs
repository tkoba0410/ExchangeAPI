using System.Collections.Generic;

namespace ExchangeApi.Spec.Wire;

public sealed record WireCallSpec(
    string Method,
    string Path,
    string? Query = null,
    string? BodyJson = null,
    IReadOnlyDictionary<string, string>? Headers = null);
