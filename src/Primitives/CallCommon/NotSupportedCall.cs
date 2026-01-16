using System;
using System.Collections.Generic;

namespace ExchangeApi.Primitives.CallCommon;

public static class NotSupportedCall
{
    private static readonly IReadOnlyDictionary<string, string> NonRetryableTags =
        new Dictionary<string, string> { ["Retryable"] = "false" };

    public static Call<TReq, TOk> Create<TReq, TOk>(
        string layer,
        string component,
        TReq request,
        string feature)
    {
        if (layer is null) throw new ArgumentNullException(nameof(layer));
        if (component is null) throw new ArgumentNullException(nameof(component));
        if (feature is null) throw new ArgumentNullException(nameof(feature));

        var now = DateTimeOffset.UtcNow;
        var meta = new CallMeta(layer, component, NonRetryableTags, null);
        var error = new CallError(CallErrorKind.Semantic, $"NotSupported:{feature}");

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }
}
