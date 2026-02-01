using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerOrderResult(
    OrderKey Key,
    string? ExchangeOrderId = null,
    string? AcceptanceId = null);
