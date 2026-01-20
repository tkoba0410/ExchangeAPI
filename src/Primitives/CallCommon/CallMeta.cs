using System;
using System.Collections.Generic;

namespace ExchangeApi.Primitives.CallCommon;

public sealed record CallMeta(
    string Layer,
    string Component,
    string EndpointId,
    IReadOnlyDictionary<string, string>? Tags = null,
    IReadOnlyList<CallId>? Children = null)
{
    public const string InternalEndpointId = "Internal";

    public string? RawJson { get; init; }

    public static CallMeta CreateInternal(
        string layer,
        string component,
        IReadOnlyDictionary<string, string>? tags = null,
        IReadOnlyList<CallId>? children = null) =>
        new(layer, component, InternalEndpointId, tags, children);
}
