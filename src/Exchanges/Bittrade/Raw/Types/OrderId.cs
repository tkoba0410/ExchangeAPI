using System;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public readonly record struct OrderId(string Value)
{
    public override string ToString() => Value;

    public static OrderId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("OrderId must not be empty.", nameof(value));
        }

        return new OrderId(value);
    }
}
