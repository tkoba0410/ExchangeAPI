using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;

internal static class SymbolMapper
{
    public static bool TryParse(string? symbol, out Symbol parsed)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            parsed = Symbol.Empty;
            return false;
        }

        parsed = new Symbol(symbol);
        return true;
    }

    public static Symbol ParseOrThrow(string symbol)
    {
        if (!TryParse(symbol, out var parsed))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        return parsed;
    }

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
        if (!Symbol.TryParse(productCode, out var parsed))
        {
            throw new ArgumentException(
                $"Bittrade symbol is invalid: '{productCode}'. Expected lowercase alphanumeric like 'btcjpy'.",
                nameof(symbol));
        }

        return parsed.Value;
    }

}
