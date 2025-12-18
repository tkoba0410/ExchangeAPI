namespace ExchangeApi.Common.Dtos;

/// <summary>
/// 抽象注文レスポンス。
/// </summary>
/// <summary>
/// 注文結果。サーバーからの OrderId（child_order_acceptance_id）を保持する。
/// </summary>
public sealed record OrderResult(string OrderId);
