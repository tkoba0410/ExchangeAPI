using System;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;

internal static class BittradeSymbolMapper
{
    public static Symbol Parse(string symbol) =>
        new Symbol(ToCanonicalSymbol(symbol));

    public static string ToProductCode(Symbol symbol)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var value = symbol.Value;
        if (value.Contains('/'))
        {
            return value.Replace("/", "_", StringComparison.Ordinal).ToUpperInvariant();
        }

        return value.ToUpperInvariant();
    }

    public static string ToApiSymbol(Symbol symbol)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = ToProductCode(symbol);
        return productCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }

    private static string ToCanonicalSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return string.Empty;
        }

        if (symbol.Contains('/'))
        {
            return symbol.ToUpperInvariant();
        }

        var upper = symbol.ToUpperInvariant();
        if (upper.EndsWith("JPY", StringComparison.Ordinal))
        {
            var basePart = upper[..^3];
            return $"{basePart}/JPY";
        }

        if (upper.Length >= 6)
        {
            var mid = upper.Length / 2;
            return $"{upper[..mid]}/{upper[mid..]}";
        }

        return upper;
    }
}
