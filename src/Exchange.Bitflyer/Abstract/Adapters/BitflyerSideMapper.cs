using System;
using ExchangeApi.Adapter.Bitflyer;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Adapter.Bitflyer.Adapters;

/// <summary>bitFlyer のサイド文字列と enum のマッピング。</summary>
internal enum BitflyerSide
{
    Buy,
    Sell,
}

internal static class BitflyerSideMapper
{
    public static BitflyerSide FromApi(string side) =>
        (side ?? string.Empty).ToUpperInvariant() switch
        {
            BitflyerConstants.Side.Buy => BitflyerSide.Buy,
            BitflyerConstants.Side.Sell => BitflyerSide.Sell,
            _ => BitflyerSide.Sell, // 互換性のためデフォルトは SELL
        };

    public static string ToApi(BitflyerSide side) =>
        side switch
        {
            BitflyerSide.Buy => BitflyerConstants.Side.Buy,
            BitflyerSide.Sell => BitflyerConstants.Side.Sell,
            _ => BitflyerConstants.Side.Buy,
        };

    public static string ToApi(OrderSide side) =>
        side switch
        {
            OrderSide.Buy => BitflyerConstants.Side.Buy,
            OrderSide.Sell => BitflyerConstants.Side.Sell,
            _ => BitflyerConstants.Side.Buy,
        };

    public static OrderSide ToOrderSide(BitflyerSide side) =>
        side switch
        {
            BitflyerSide.Buy => OrderSide.Buy,
            BitflyerSide.Sell => OrderSide.Sell,
            _ => OrderSide.Buy,
        };

    public static OrderSide ToOrderSide(string side) => ToOrderSide(FromApi(side));
}
