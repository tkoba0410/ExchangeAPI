using System;

namespace ExchangeApi.Primitives.CallCommon;

public readonly record struct CallId(Guid Value)
{
    public static CallId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("N");
}
