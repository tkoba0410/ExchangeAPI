namespace ExchangeApi.Contracts.Dtos;

/// <summary>
/// 抽象注文リクエスト。
/// <remarks>
/// 必須組み合わせ: MARKET は Size のみ必須、LIMIT は Price を必須、STOP/STOP_LIMIT は TriggerPrice を必須（STOP_LIMIT は Price も必須）。
/// TimeInForce は LIMIT/STOP_LIMIT のみ有効で、各取引所のサポート範囲に従う。
/// ClientOrderId は重複不可を想定し、アダプターは未対応の場合は無視またはエラーに正規化する。
/// PriceIncrement/SizeIncrement/MinSize/MaxSize/MinNotional は ExchangeInfo 由来のバリデーションヒントであり、欠損時は取引所デフォルトに従う。
/// </remarks>
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
    TimeInForce? TimeInForce = null,
    decimal? PriceIncrement = null,
    decimal? SizeIncrement = null,
    decimal? MinSize = null,
    decimal? MaxSize = null,
    decimal? MinNotional = null);
