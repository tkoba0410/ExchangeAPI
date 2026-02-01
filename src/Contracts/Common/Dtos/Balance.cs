using ExchangeApi.Primitives.DomainCommon.Enums;
namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>口座残高（通貨のみ型安全）。</summary>
public sealed record Balance(
    string Currency,
    decimal Amount,
    decimal Available,
    CurrencyCode CurrencyCode = CurrencyCode.Unknown);
