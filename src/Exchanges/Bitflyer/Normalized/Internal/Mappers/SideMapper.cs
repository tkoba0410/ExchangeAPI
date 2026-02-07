using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

/// <summary>bitFlyer のサイド文字列と enum のマッピング。</summary>
internal static class SideMapper
{
    public static bool TryToApi(ExchangeSide side, out string apiSide, out CallError? error)
    {
        switch (side)
        {
            case ExchangeSide.Buy:
                apiSide = "BUY";
                error = null;
                return true;
            case ExchangeSide.Sell:
                apiSide = "SELL";
                error = null;
                return true;
            default:
                apiSide = string.Empty;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer side: {side}.");
                return false;
        }
    }

    public static bool TryToExchangeSide(string side, out ExchangeSide parsed, out CallError? error)
    {
        switch ((side ?? string.Empty).ToUpperInvariant())
        {
            case "BUY":
                parsed = ExchangeSide.Buy;
                error = null;
                return true;
            case "SELL":
                parsed = ExchangeSide.Sell;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer side: {side ?? "<null>"}.");
                return false;
        }
    }

    public static bool TryToContractSide(ExchangeSide side, out ContractSide contractSide, out CallError? error)
    {
        switch (side)
        {
            case ExchangeSide.Buy:
                contractSide = ContractSide.Buy;
                error = null;
                return true;
            case ExchangeSide.Sell:
                contractSide = ContractSide.Sell;
                error = null;
                return true;
            default:
                contractSide = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer side: {side}.");
                return false;
        }
    }

    public static bool TryFromContractSide(ContractSide side, out ExchangeSide parsed, out CallError? error)
    {
        switch (side)
        {
            case ContractSide.Buy:
                parsed = ExchangeSide.Buy;
                error = null;
                return true;
            case ContractSide.Sell:
                parsed = ExchangeSide.Sell;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown contract side: {side}.");
                return false;
        }
    }
}
