using ExchangeApi.Common.Types;

namespace ExchangeApi.Common.Dtos;

/// <summary>
/// 抽象注文レスポンス。
/// </summary>
/// <summary>
/// 注文結果。
/// </summary>
public sealed record OrderResult(
    OrderKey Key,
    string? ExchangeOrderId = null,
    string? AcceptanceId = null);
