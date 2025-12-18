using System;
using ExchangeApi.Common.Enums;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Raw.Side;
using ContractSide = ExchangeApi.Common.Enums.Side;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

/// <summary>bitFlyer のサイド文字列と enum のマッピング。</summary>
internal static class BitflyerSideMapper
{
    public static RawSide FromApi(string side) =>
        (side ?? string.Empty).ToUpperInvariant() switch
        {
            "BUY" => RawSide.Buy,
            "SELL" => RawSide.Sell,
            _ => RawSide.Sell, // 互換性のためデフォルトは SELL
        };

    public static string ToApi(RawSide side) =>
        side switch
        {
            RawSide.Buy => "BUY",
            RawSide.Sell => "SELL",
            _ => "BUY",
        };

    public static string ToApi(ContractSide side) =>
        side switch
        {
            ContractSide.Buy => "BUY",
            ContractSide.Sell => "SELL",
            _ => "BUY",
        };

    public static ContractSide ToOrderSide(RawSide side) =>
        side switch
        {
            RawSide.Buy => ContractSide.Buy,
            RawSide.Sell => ContractSide.Sell,
            _ => ContractSide.Buy,
        };

    public static RawSide ToRawSide(ContractSide side) =>
        side switch
        {
            ContractSide.Buy => RawSide.Buy,
            ContractSide.Sell => RawSide.Sell,
            _ => RawSide.Buy,
        };

    public static ContractSide ToOrderSide(string side) => ToOrderSide(FromApi(side));
}
