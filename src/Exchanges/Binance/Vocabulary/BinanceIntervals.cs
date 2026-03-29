using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Binance.Vocabulary;

[JsonConverter(typeof(BinanceApiStringEnumJsonConverter<BinanceInterval>))]
public enum BinanceInterval
{
    [BinanceApiStringEnumValue("1s")]
    Second1s = 1,
    [BinanceApiStringEnumValue("1m")]
    Minute1m = 2,
    [BinanceApiStringEnumValue("3m")]
    Minute3m = 3,
    [BinanceApiStringEnumValue("5m")]
    Minute5m = 4,
    [BinanceApiStringEnumValue("15m")]
    Minute15m = 5,
    [BinanceApiStringEnumValue("30m")]
    Minute30m = 6,
    [BinanceApiStringEnumValue("1h")]
    Hour1h = 7,
    [BinanceApiStringEnumValue("2h")]
    Hour2h = 8,
    [BinanceApiStringEnumValue("4h")]
    Hour4h = 9,
    [BinanceApiStringEnumValue("6h")]
    Hour6h = 10,
    [BinanceApiStringEnumValue("8h")]
    Hour8h = 11,
    [BinanceApiStringEnumValue("12h")]
    Hour12h = 12,
    [BinanceApiStringEnumValue("1d")]
    Day1d = 13,
    [BinanceApiStringEnumValue("3d")]
    Day3d = 14,
    [BinanceApiStringEnumValue("1w")]
    Week1w = 15,
    [BinanceApiStringEnumValue("1M")]
    Month1M = 16,
}

public static class BinanceIntervals
{
    public const BinanceInterval Second1s = BinanceInterval.Second1s;
    public const BinanceInterval Minute1m = BinanceInterval.Minute1m;
    public const BinanceInterval Minute3m = BinanceInterval.Minute3m;
    public const BinanceInterval Minute5m = BinanceInterval.Minute5m;
    public const BinanceInterval Minute15m = BinanceInterval.Minute15m;
    public const BinanceInterval Minute30m = BinanceInterval.Minute30m;
    public const BinanceInterval Hour1h = BinanceInterval.Hour1h;
    public const BinanceInterval Hour2h = BinanceInterval.Hour2h;
    public const BinanceInterval Hour4h = BinanceInterval.Hour4h;
    public const BinanceInterval Hour6h = BinanceInterval.Hour6h;
    public const BinanceInterval Hour8h = BinanceInterval.Hour8h;
    public const BinanceInterval Hour12h = BinanceInterval.Hour12h;
    public const BinanceInterval Day1d = BinanceInterval.Day1d;
    public const BinanceInterval Day3d = BinanceInterval.Day3d;
    public const BinanceInterval Week1w = BinanceInterval.Week1w;
    public const BinanceInterval Month1M = BinanceInterval.Month1M;
}
