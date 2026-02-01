using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;

internal static class BittradeSymbolMapper
{
    public static Symbol Parse(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        return new Symbol(symbol);
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
