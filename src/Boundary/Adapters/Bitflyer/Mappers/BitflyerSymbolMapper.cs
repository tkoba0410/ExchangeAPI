using System;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.Types.RawProductCode;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;

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
