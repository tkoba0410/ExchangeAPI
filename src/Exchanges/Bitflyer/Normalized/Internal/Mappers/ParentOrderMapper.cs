using System;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class ParentOrderMapper
{
    public static bool TryToApiOrderMethod(OrderMethod method, out string apiMethod, out CallError? error)
    {
        apiMethod = method switch
        {
            OrderMethod.Simple => "SIMPLE",
            OrderMethod.Ifd => "IFD",
            OrderMethod.Oco => "OCO",
            OrderMethod.IfdOco => "IFDOCO",
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

    public static bool TryToApiConditionType(ConditionType type, out string apiType, out CallError? error)
    {
        apiType = type switch
        {
            ConditionType.Limit => "LIMIT",
            ConditionType.Market => "MARKET",
            ConditionType.Stop => "STOP",
            ConditionType.StopLimit => "STOP_LIMIT",
            ConditionType.Trail => "TRAIL",
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

    public static bool TryToApiTimeInForce(TimeInForce tif, out string apiTif, out CallError? error)
    {
        apiTif = tif switch
        {
            TimeInForce.Gtc => "GTC",
            TimeInForce.Ioc => "IOC",
            TimeInForce.Fok => "FOK",
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

    public static bool TryToApiParentOrderState(ParentOrderState state, out string apiState, out CallError? error)
    {
        apiState = state switch
        {
            ParentOrderState.Active => "ACTIVE",
            ParentOrderState.Completed => "COMPLETED",
            ParentOrderState.Canceled => "CANCELED",
            ParentOrderState.Expired => "EXPIRED",
            ParentOrderState.Rejected => "REJECTED",
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
