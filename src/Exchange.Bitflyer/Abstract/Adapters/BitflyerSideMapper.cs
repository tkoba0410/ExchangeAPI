using System;
using Exchange.Bitflyer.Raw;
using Common.Contract.Dtos;
using Common.Contract.Enums;

namespace Exchange.Bitflyer.Abstract;

/// <summary>bitFlyer のサイド文字列と enum のマッピング。</summary>
internal static class BitflyerSideMapper
{
    public static Side FromApi(string side) =>
        (side ?? string.Empty).ToUpperInvariant() switch
        {
            "BUY" => Side.Buy,
            "SELL" => Side.Sell,
            _ => Side.Sell, // 互換性のためデフォルトは SELL
        };

    public static string ToApi(Side side) =>
        side switch
        {
            Side.Buy => "BUY",
            Side.Sell => "SELL",
            _ => "BUY",
        };

    public static string ToApi(OrderSide side) =>
        side switch
        {
            OrderSide.Buy => "BUY",
            OrderSide.Sell => "SELL",
            _ => "BUY",
        };

    public static OrderSide ToOrderSide(Side side) =>
        side switch
        {
            Side.Buy => OrderSide.Buy,
            Side.Sell => OrderSide.Sell,
            _ => OrderSide.Buy,
        };

    public static Side ToRawSide(OrderSide side) =>
        side switch
        {
            OrderSide.Buy => Side.Buy,
            OrderSide.Sell => Side.Sell,
            _ => Side.Buy,
        };

    public static OrderSide ToOrderSide(string side) => ToOrderSide(FromApi(side));
}
