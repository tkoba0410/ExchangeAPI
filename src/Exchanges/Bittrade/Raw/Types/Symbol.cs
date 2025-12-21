using System;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct Symbol(string Value)
{
    public override string ToString() => Value;

    public static Symbol From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Symbol must not be empty.", nameof(value));
        }

        return new Symbol(value);
    }
}
