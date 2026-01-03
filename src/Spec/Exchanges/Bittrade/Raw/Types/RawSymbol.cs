using System;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Types;

public readonly record struct RawSymbol(string Value)
{
    public override string ToString() => Value;

    public static RawSymbol From(string value)
    {
        return new RawSymbol(value);
    }
}
