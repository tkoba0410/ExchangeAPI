using System;
using System.Collections.Generic;

namespace ExchangeApi.Spec.CallCommon;

public sealed record CallMeta(
    string Layer,
    string Component,
    IReadOnlyDictionary<string, string>? Tags = null,
    IReadOnlyList<CallId>? Children = null)
{
    public string? RawJson { get; init; }
}
