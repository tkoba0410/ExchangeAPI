namespace Common.Contract.Dtos;

/// <summary>
/// 注文ステータスの簡易ビュー（ポーリング用）。
/// </summary>
public sealed record OrderStatusSnapshot(
    string ProductCode,
    string OrderAcceptanceId,
    OrderStatus Status,
    decimal ExecutedSize,
    decimal OutstandingSize,
    decimal? Price,
    decimal? AveragePrice);

public enum OrderStatus
{
    Unknown = 0,
    Active = 1,
    Completed = 2,
    Canceled = 3,
    Expired = 4,
    Rejected = 5
}
