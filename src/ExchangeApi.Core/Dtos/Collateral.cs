namespace ExchangeApi.Core.Dtos;

/// <summary>
/// 証拠金情報。
/// </summary>
public sealed record Collateral(
    decimal Amount,
    decimal OpenPositionPnl,
    decimal RequireCollateral,
    decimal KeepRate);
