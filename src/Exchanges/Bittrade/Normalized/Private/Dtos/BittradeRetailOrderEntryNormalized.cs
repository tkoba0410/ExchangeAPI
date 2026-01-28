using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeRetailOrderEntryNormalized(
    string Id,
    string Symbol,
    int Type,
    decimal? Price,
    decimal? Amount,
    decimal? CashAmount,
    int? Status,
    DateTimeOffset? CreatedAt);
