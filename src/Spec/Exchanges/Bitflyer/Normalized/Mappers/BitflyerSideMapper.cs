using System;
using ExchangeApi.Common.Enums;
using ContractSide = ExchangeApi.Common.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

/// <summary>bitFlyer のサイド文字列と enum のマッピング。</summary>
internal static class BitflyerSideMapper
{
    public static string ToApi(ContractSide side) =>
        side switch
        {
            ContractSide.Buy => "BUY",
            ContractSide.Sell => "SELL",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown bitFlyer side"),
        };

    public static ContractSide ToOrderSide(string side) =>
        (side ?? string.Empty).ToUpperInvariant() switch
        {
            "BUY" => ContractSide.Buy,
            "SELL" => ContractSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown bitFlyer side"),
        };
}
