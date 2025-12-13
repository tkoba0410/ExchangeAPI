using System;
using Exchange.Bitflyer.Raw;
using ExchangeApi.Contracts.Dtos;

namespace Exchange.Bitflyer.Abstract;

/// <summary>bitFlyer のサイド文字列と enum のマッピング。</summary>
internal static class BitflyerSideMapper
{
    public static Side FromApi(string side) =>
        (side ?? string.Empty).ToUpperInvariant() switch
        {
            BitflyerConstants.Side.Buy => Side.Buy,
            BitflyerConstants.Side.Sell => Side.Sell,
            _ => Side.Sell, // 互換性のためデフォルトは SELL
        };

    public static string ToApi(Side side) =>
        side switch
        {
            Side.Buy => BitflyerConstants.Side.Buy,
            Side.Sell => BitflyerConstants.Side.Sell,
            _ => BitflyerConstants.Side.Buy,
        };

    public static string ToApi(OrderSide side) =>
        side switch
        {
            OrderSide.Buy => BitflyerConstants.Side.Buy,
            OrderSide.Sell => BitflyerConstants.Side.Sell,
            _ => BitflyerConstants.Side.Buy,
        };

    public static OrderSide ToOrderSide(Side side) =>
        side switch
        {
            Side.Buy => OrderSide.Buy,
            Side.Sell => OrderSide.Sell,
            _ => OrderSide.Buy,
        };

    public static OrderSide ToOrderSide(string side) => ToOrderSide(FromApi(side));
}
