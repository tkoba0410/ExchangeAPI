using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

/// <summary>bitFlyer のサイド文字列と enum のマッピング。</summary>
internal static class BitflyerSideMapper
{
    public static string ToApi(BitflyerSide side) =>
        side switch
        {
            BitflyerSide.Buy => "BUY",
            BitflyerSide.Sell => "SELL",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown bitFlyer side"),
        };

    public static BitflyerSide ToExchangeSide(string side) =>
        (side ?? string.Empty).ToUpperInvariant() switch
        {
            "BUY" => BitflyerSide.Buy,
            "SELL" => BitflyerSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown bitFlyer side"),
        };

    public static ContractSide ToContractSide(BitflyerSide side) =>
        side switch
        {
            BitflyerSide.Buy => ContractSide.Buy,
            BitflyerSide.Sell => ContractSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown bitFlyer side"),
        };

    public static BitflyerSide FromContractSide(ContractSide side) =>
        side switch
        {
            ContractSide.Buy => BitflyerSide.Buy,
            ContractSide.Sell => BitflyerSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown bitFlyer side"),
        };

    public static bool TryToApi(BitflyerSide side, out string apiSide, out CallError? error)
    {
        switch (side)
        {
            case BitflyerSide.Buy:
                apiSide = "BUY";
                error = null;
                return true;
            case BitflyerSide.Sell:
                apiSide = "SELL";
                error = null;
                return true;
            default:
                apiSide = string.Empty;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer side: {side}.");
                return false;
        }
    }

    public static bool TryToExchangeSide(string side, out BitflyerSide parsed, out CallError? error)
    {
        switch ((side ?? string.Empty).ToUpperInvariant())
        {
            case "BUY":
                parsed = BitflyerSide.Buy;
                error = null;
                return true;
            case "SELL":
                parsed = BitflyerSide.Sell;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer side: {side ?? "<null>"}.");
                return false;
        }
    }

    public static bool TryToContractSide(BitflyerSide side, out ContractSide contractSide, out CallError? error)
    {
        switch (side)
        {
            case BitflyerSide.Buy:
                contractSide = ContractSide.Buy;
                error = null;
                return true;
            case BitflyerSide.Sell:
                contractSide = ContractSide.Sell;
                error = null;
                return true;
            default:
                contractSide = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer side: {side}.");
                return false;
        }
    }

    public static bool TryFromContractSide(ContractSide side, out BitflyerSide parsed, out CallError? error)
    {
        switch (side)
        {
            case ContractSide.Buy:
                parsed = BitflyerSide.Buy;
                error = null;
                return true;
            case ContractSide.Sell:
                parsed = BitflyerSide.Sell;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown contract side: {side}.");
                return false;
        }
    }
}
