using System;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Types;

public readonly record struct RawOrderId(string Value)
{
    public override string ToString() => Value;

    public static RawOrderId From(string value)
    {
        return new RawOrderId(value);
    }
}
