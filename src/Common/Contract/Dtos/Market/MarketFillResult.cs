namespace Common.Contract.Dtos;

/// <summary>成行計算の結果。</summary>
public sealed record MarketFillResult(
    bool Filled,
    decimal TotalSize,
    decimal TotalValue,
    decimal? AveragePrice);
