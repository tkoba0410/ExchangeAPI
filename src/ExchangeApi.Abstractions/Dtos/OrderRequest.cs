namespace ExchangeApi.Abstractions.Dtos;

/// <summary>
/// 抽象注文リクエスト。
/// Stage3 では Market 注文のみを対象とする。
/// </summary>
public sealed record OrderRequest(
    string ProductCode,
    OrderSide Side,
    OrderType OrderType,
    decimal Size);

