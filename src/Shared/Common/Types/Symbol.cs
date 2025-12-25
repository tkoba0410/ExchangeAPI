using System;
using System.Text.Json.Serialization;

namespace ExchangeApi.Common.Types;

[JsonConverter(typeof(SymbolJsonConverter))]
public readonly record struct Symbol
{
    public Symbol(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static Symbol Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static Symbol Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Symbol value is required.", nameof(value));
        }

        return new Symbol(value);
    }

    public static bool TryParse(string? value, out Symbol symbol)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            symbol = Empty;
            return false;
        }

        symbol = new Symbol(value);
        return true;
    }
}
