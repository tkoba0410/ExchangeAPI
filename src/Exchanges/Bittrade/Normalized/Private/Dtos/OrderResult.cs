using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record OrderResult(
    OrderKey Key,
    ExchangeOrderId? ExchangeOrderId = null,
    AcceptanceId? AcceptanceId = null);
