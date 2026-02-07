using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class CommonMapper
{
    public static OrderState MapOrderStatus(string childOrderStatusState) =>
        (childOrderStatusState ?? string.Empty).ToUpperInvariant() switch
        {
            "ACTIVE" => OrderState.Active,
            "COMPLETED" => OrderState.Completed,
            "CANCELED" => OrderState.Canceled,
            "EXPIRED" => OrderState.Expired,
            _ => OrderState.Unknown,
        };

    public static bool TryMapSide(string side, out ContractSide mapped, out CallError? error)
    {
        if (!SideMapper.TryToExchangeSide(side, out var exchangeSide, out error))
        {
            mapped = default;
            return false;
        }

        if (!SideMapper.TryToContractSide(exchangeSide, out mapped, out error))
        {
            return false;
        }

        error = null;
        return true;
    }

    public static bool TryMapSide(ExchangeSide side, out ContractSide mapped, out CallError? error)
    {
        if (!SideMapper.TryToContractSide(side, out mapped, out error))
        {
            return false;
        }

        error = null;
        return true;
    }

    public static bool TryMapSideToExchange(ContractSide side, out string apiSide, out CallError? error)
    {
        if (!SideMapper.TryFromContractSide(side, out var exchangeSide, out error))
        {
            apiSide = string.Empty;
            return false;
        }

        if (!SideMapper.TryToApi(exchangeSide, out apiSide, out error))
        {
            return false;
        }

        error = null;
        return true;
    }

    public static bool TryToApiProductCode(string productCode, out string apiProductCode, out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            apiProductCode = string.Empty;
            error = new CallError(CallErrorKind.Mapping, $"SymbolNotSupported:{productCode ?? string.Empty}");
            return false;
        }

        apiProductCode = productCode;
        error = null;
        return true;
    }

    public static bool TryParseProductCode(string productCode, out string parsed, out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            parsed = string.Empty;
            error = new CallError(CallErrorKind.Mapping, $"SymbolNotSupported:{productCode ?? string.Empty}");
            return false;
        }

        parsed = productCode;
        error = null;
        return true;
    }
}
