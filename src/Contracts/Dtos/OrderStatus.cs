using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Dtos;

/// <summary>注文ステータスの簡易ビュー（ポーリング用）。</summary>
public sealed record OrderStatus(
    string ProductCode,
    OrderKey Key,
    OrderState Status,
    Size ExecutedSize,
    Size OutstandingSize,
    Price? Price,
    Price? AveragePrice);
