using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Common.Dtos;

/// <summary>注文ステータスの簡易ビュー（ポーリング用）。</summary>
public sealed record OrderStatus(
    string ProductCode,
    OrderKey Key,
    OrderState Status,
    Size ExecutedSize,
    Size OutstandingSize,
    Price? Price,
    Price? AveragePrice);
