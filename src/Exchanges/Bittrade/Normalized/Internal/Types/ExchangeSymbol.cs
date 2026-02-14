using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;

public readonly record struct ExchangeSymbol(string Value)
{
    public override string ToString() => Value;

    public static bool TryParse(string? value, out ExchangeSymbol symbol)
    {
        if (!TryNormalize(value, out var normalized))
        {
            symbol = default;
            return false;
        }

        symbol = new ExchangeSymbol(normalized);
        return true;
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
