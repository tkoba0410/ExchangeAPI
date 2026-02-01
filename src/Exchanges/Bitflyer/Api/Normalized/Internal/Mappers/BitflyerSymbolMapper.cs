using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerSymbolMapper
{
    public static string ToProductCode(Symbol symbol)
    {
        if (symbol.IsEmpty)
        {
            throw new InvalidOperationException($"SymbolNotSupported:{symbol}");
        }

        return ToProductCode(symbol.Value);
    }

    public static string ToProductCode(string symbol)
    {
        return BitflyerCommonMapper.ParseProductCode(symbol);
    }

    public static string ToApiProductCode(string productCode) =>
        BitflyerCommonMapper.ToApiProductCode(productCode);

    public static Symbol FromProductCode(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new InvalidOperationException($"SymbolNotSupported:{symbol ?? string.Empty}");
        }

        return new Symbol(symbol);
    }
}
