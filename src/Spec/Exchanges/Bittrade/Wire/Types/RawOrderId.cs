using System;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Types;

public readonly record struct RawOrderId(string Value)
{
    public override string ToString() => Value;

    public static RawOrderId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("RawOrderId must not be empty.", nameof(value));
        }

        return new RawOrderId(value);
    }
}
