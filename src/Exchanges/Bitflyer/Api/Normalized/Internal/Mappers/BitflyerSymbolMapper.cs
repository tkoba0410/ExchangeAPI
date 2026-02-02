using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.CallCommon;

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

    public static bool TryToProductCode(Symbol symbol, out string productCode, out CallError? error)
    {
        if (symbol.IsEmpty)
        {
            productCode = string.Empty;
            error = new CallError(CallErrorKind.Mapping, $"SymbolNotSupported:{symbol}");
            return false;
        }

        return BitflyerCommonMapper.TryParseProductCode(symbol.Value, out productCode, out error);
    }

    public static bool TryToProductCode(string symbol, out string productCode, out CallError? error)
    {
        return BitflyerCommonMapper.TryParseProductCode(symbol, out productCode, out error);
    }

    public static bool TryToApiProductCode(string productCode, out string apiProductCode, out CallError? error)
    {
        return BitflyerCommonMapper.TryToApiProductCode(productCode, out apiProductCode, out error);
    }

    public static bool TryFromProductCode(string symbol, out Symbol parsed, out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            parsed = default;
            error = new CallError(CallErrorKind.Mapping, $"SymbolNotSupported:{symbol ?? string.Empty}");
            return false;
        }

        parsed = new Symbol(symbol);
        error = null;
        return true;
    }
}
