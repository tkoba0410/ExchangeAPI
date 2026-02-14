using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class SymbolMapper
{
    public static bool TryToProductCode(Symbol symbol, out ProductCode productCode, out CallError? error)
    {
        if (symbol.IsEmpty)
        {
            productCode = ProductCode.Empty;
            error = new CallError(CallErrorKind.Mapping, $"SymbolNotSupported:{symbol}");
            return false;
        }

        var parsed = ProductCode.Parse(symbol.Value);
        return CommonMapper.TryParseProductCode(parsed, out productCode, out error);
    }

    public static bool TryToApiProductCode(ProductCode productCode, out ProductCode apiProductCode, out CallError? error)
    {
        return CommonMapper.TryToApiProductCode(productCode, out apiProductCode, out error);
    }

    public static bool TryFromProductCode(ProductCode productCode, out Symbol parsed, out CallError? error)
    {
        if (productCode.IsEmpty)
        {
            parsed = default;
            error = new CallError(CallErrorKind.Mapping, "SymbolNotSupported:<empty>");
            return false;
        }

        parsed = new Symbol(productCode.Value);
        error = null;
        return true;
    }
}
