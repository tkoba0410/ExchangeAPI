using System;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Wire.Types.RawProductCode;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerSymbolMapper
{
    public static RawProductCode ToProductCode(Symbol symbol)
    {
        if (symbol.IsEmpty)
        {
            throw new SymbolNotSupportedException(symbol.ToString());
        }

        return ToProductCode(symbol.Value);
    }

    public static RawProductCode ToProductCode(string symbol)
    {
        return BitflyerCommonMapper.ParseProductCode(symbol);
    }

    public static string ToApiProductCode(RawProductCode productCode) =>
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
