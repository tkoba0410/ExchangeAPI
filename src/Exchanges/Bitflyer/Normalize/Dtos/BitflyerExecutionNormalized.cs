using System;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerExecutionNormalized(
    long Id,
    string Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt,
    string? ChildOrderAcceptanceId);
