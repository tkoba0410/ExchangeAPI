using Common.Enums;
namespace Common.Dtos;

/// <summary>
/// 証拠金情報。
/// </summary>
public sealed record Collateral(
    ExchangeCode ExchangeCode,
    string Currency,
    decimal Amount,
    decimal OpenPositionPnl,
    decimal RequireCollateral,
    decimal KeepRate);
