using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Types;

public readonly record struct BittradeSymbol(string Value)
{
    public override string ToString() => Value;

    public static bool TryParse(string? value, out BittradeSymbol symbol)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            symbol = default;
            return false;
        }

        var trimmed = value.Trim();
        for (var i = 0; i < trimmed.Length; i++)
        {
            var ch = trimmed[i];
            if (!char.IsLetterOrDigit(ch))
            {
                symbol = default;
                return false;
            }
        }

        symbol = new BittradeSymbol(trimmed.ToLowerInvariant());
        return true;
    }

    public static BittradeSymbol ParseOrThrow(string? value)
    {
        if (TryParse(value, out var symbol))
        {
            return symbol;
        }

        throw new ArgumentException(
            $"Bittrade symbol is invalid: '{value}'. Expected lowercase alphanumeric like 'btcjpy'.",
            nameof(value));
    }
}
