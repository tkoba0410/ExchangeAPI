namespace ExchangeApi.Abstractions.Dtos;

/// <summary>
/// 抽象注文リクエスト。
/// </summary>
public sealed record OrderRequest(
    string ProductCode,
    OrderSide Side,
    OrderType OrderType,
    decimal Size,
    string? ClientOrderId = null,
    decimal? Price = null,
    decimal? TriggerPrice = null,
    int? MinuteToExpire = null,
    TimeInForce? TimeInForce = null);
