using ExchangeApi.Primitives.DomainCommon.Enums;
namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>口座残高。</summary>
public sealed record Balance(
    CurrencyCode Currency,
    decimal Amount,
    decimal Available);
