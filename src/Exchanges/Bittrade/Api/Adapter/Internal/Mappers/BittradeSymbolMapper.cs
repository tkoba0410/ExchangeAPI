using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;

internal static class BittradeSymbolMapper
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
        return BittradeSymbol.Normalize(productCode);
    }

}
