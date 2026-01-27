using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Types;

public readonly record struct BittradeSymbol(string Value)
{
    public override string ToString() => Value;

    public static bool TryParse(string? value, out BittradeSymbol symbol)
    {
        if (!TryNormalize(value, out var normalized))
        {
            symbol = default;
            return false;
        }

        symbol = new BittradeSymbol(normalized);
        return true;
    }

    public static string Normalize(string? value)
    {
        if (TryNormalize(value, out var normalized))
        {
            return normalized;
        }

        throw new ArgumentException(
            $"Bittrade symbol is invalid: '{value}'. Expected lowercase alphanumeric like 'btcjpy'.",
            nameof(value));
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

    private static bool TryNormalize(string? value, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            return false;
        }

        var buffer = new char[trimmed.Length];
        var length = 0;

        for (var i = 0; i < trimmed.Length; i++)
        {
            var ch = trimmed[i];
            if (ch is '_' or '/')
            {
                continue;
            }

            if (!char.IsLetterOrDigit(ch))
            {
                return false;
            }

            buffer[length++] = char.ToLowerInvariant(ch);
        }

        if (length == 0)
        {
            return false;
        }

        normalized = new string(buffer, 0, length);
        return true;
    }
}
