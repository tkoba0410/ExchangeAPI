using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>
/// 抽象注文レスポンス。
/// </summary>
/// <summary>
/// 注文結果。
/// </summary>
public sealed record OrderResult(
    OrderKey Key,
    ExchangeOrderId? ExchangeOrderId = null,
    AcceptanceId? AcceptanceId = null);
