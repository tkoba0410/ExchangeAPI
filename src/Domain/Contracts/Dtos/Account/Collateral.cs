using ExchangeApi.Common.Enums;
namespace ExchangeApi.Contracts.Dtos;

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
