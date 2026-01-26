using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;

public sealed record BittradeOrderSummaryNormalized(
    string Id,
    string Symbol,
    string AccountId,
    decimal Amount,
    decimal? Price,
    string State,
    string Type,
    string? ClientOrderId,
    DateTimeOffset CreatedAt,
    decimal FilledAmount);
