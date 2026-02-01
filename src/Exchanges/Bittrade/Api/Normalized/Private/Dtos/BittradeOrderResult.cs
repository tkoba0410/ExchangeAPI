using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeOrderResult(
    OrderKey Key,
    string? ExchangeOrderId = null,
    string? AcceptanceId = null);
