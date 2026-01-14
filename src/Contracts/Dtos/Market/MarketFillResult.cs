using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Dtos.Market;

/// <summary>約定見積（フィル見積）。</summary>
public sealed record FillEstimate(
    bool IsFilled,                // 目標を満たしたか（部分約定の場合は false）
    Size SignedSize,              // 見積もり約定サイズ（買い=正、売り=負）
    decimal Delta,                // 見積もり約定金額（買い=支払=正、売り=受取=負）
    Price? EstimatedAveragePrice, // 見積もり平均約定価格（サイズが0のとき null）
    Price? TargetPrice,           // 価格条件（価格基準のとき設定、サイズ基準では null）
    Size? TargetSize);            // サイズ目標（サイズ基準のとき設定、価格基準では null）
