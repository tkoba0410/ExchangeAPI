using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Errors;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerSymbolMapper
{
    public static string ToProductCode(Symbol symbol)
    {
        if (symbol.IsEmpty)
        {
            throw new SymbolNotSupportedException(symbol.ToString());
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
            throw new SymbolNotSupportedException(symbol ?? string.Empty);
        }

        return new Symbol(symbol);
    }
}
