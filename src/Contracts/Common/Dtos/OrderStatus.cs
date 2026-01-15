using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>注文ステータスの簡易ビュー（ポーリング用）。</summary>
public sealed record OrderStatus(
    string ProductCode,
    OrderKey Key,
    OrderState Status,
    Size ExecutedSize,
    Size OutstandingSize,
    Price? Price,
    Price? AveragePrice);
