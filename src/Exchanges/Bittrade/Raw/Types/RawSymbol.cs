using System;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct RawSymbol(string Value)
{
    public override string ToString() => Value;

    public static RawSymbol From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("RawSymbol must not be empty.", nameof(value));
        }

        return new RawSymbol(value);
    }
}
