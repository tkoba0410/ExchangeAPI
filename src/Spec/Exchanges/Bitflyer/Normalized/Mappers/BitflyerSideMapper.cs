using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;
using ContractSide = ExchangeApi.Common.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

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
}
