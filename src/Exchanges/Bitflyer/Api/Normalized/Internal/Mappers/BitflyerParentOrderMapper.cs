using System;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerParentOrderMapper
{
    public static bool TryToApiOrderMethod(BitflyerOrderMethod method, out string apiMethod, out CallError? error)
    {
        apiMethod = method switch
        {
            BitflyerOrderMethod.Simple => "SIMPLE",
            BitflyerOrderMethod.Ifd => "IFD",
            BitflyerOrderMethod.Oco => "OCO",
            BitflyerOrderMethod.IfdOco => "IFDOCO",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(apiMethod))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer order_method: {method}.");
            return false;
        }

        error = null;
        return true;
    }

    public static bool TryToApiConditionType(BitflyerConditionType type, out string apiType, out CallError? error)
    {
        apiType = type switch
        {
            BitflyerConditionType.Limit => "LIMIT",
            BitflyerConditionType.Market => "MARKET",
            BitflyerConditionType.Stop => "STOP",
            BitflyerConditionType.StopLimit => "STOP_LIMIT",
            BitflyerConditionType.Trail => "TRAIL",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(apiType))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer condition_type: {type}.");
            return false;
        }

        error = null;
        return true;
    }

    public static bool TryToApiTimeInForce(BitflyerTimeInForce tif, out string apiTif, out CallError? error)
    {
        apiTif = tif switch
        {
            BitflyerTimeInForce.Gtc => "GTC",
            BitflyerTimeInForce.Ioc => "IOC",
            BitflyerTimeInForce.Fok => "FOK",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(apiTif))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer time_in_force: {tif}.");
            return false;
        }

        error = null;
        return true;
    }

    public static bool TryToApiParentOrderState(BitflyerParentOrderState state, out string apiState, out CallError? error)
    {
        apiState = state switch
        {
            BitflyerParentOrderState.Active => "ACTIVE",
            BitflyerParentOrderState.Completed => "COMPLETED",
            BitflyerParentOrderState.Canceled => "CANCELED",
            BitflyerParentOrderState.Expired => "EXPIRED",
            BitflyerParentOrderState.Rejected => "REJECTED",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(apiState))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer parent_order_state: {state}.");
            return false;
        }

        error = null;
        return true;
    }
}
