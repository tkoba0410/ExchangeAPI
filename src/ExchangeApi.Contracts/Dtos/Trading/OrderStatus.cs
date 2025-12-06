namespace ExchangeApi.Contracts.Dtos;

/// <summary>
/// 注文ステータスの簡易ビュー（ポーリング用）。
/// </summary>
public sealed record OrderStatus(
    string ProductCode,
    string OrderAcceptanceId,
    OrderStatusType Status,
    decimal ExecutedSize,
    decimal OutstandingSize,
    decimal? Price,
    decimal? AveragePrice);

public enum OrderStatusType
{
    Unknown = 0,
    Active = 1,
    Completed = 2,
    Canceled = 3,
    Expired = 4
}
