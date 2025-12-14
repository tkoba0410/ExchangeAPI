namespace Common.Contract.Dtos;

/// <summary>約定見積（フィル見積）。</summary>
public sealed record FillEstimate(
    bool IsFilled,        // 目標を満たしたか（部分約定の場合は false）
    decimal ExecutedSize, // 約定したサイズ（見積）
    decimal ExecutedValue,// 約定した金額（支払/受取の総額）
    decimal? AveragePrice // 加重平均約定価格（サイズが0のとき null）
);
