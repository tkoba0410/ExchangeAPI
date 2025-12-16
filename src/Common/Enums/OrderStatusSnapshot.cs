namespace Common.Enums;

/// <summary>注文ステータスの簡易ビュー（ポーリング用）。</summary>
public sealed record OrderStatus(
    string ProductCode,
    string OrderAcceptanceId,
    OrderState Status,
    decimal ExecutedSize,
    decimal OutstandingSize,
    decimal? Price,
    decimal? AveragePrice);

public enum OrderState
{
    Unknown = 0,
    Active = 1,
    Completed = 2,
    Canceled = 3,
    Expired = 4,
    Rejected = 5
}
