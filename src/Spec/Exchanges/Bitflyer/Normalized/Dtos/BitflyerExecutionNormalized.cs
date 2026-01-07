using System;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerExecutionNormalized(
    long Id,
    BitflyerSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt,
    string? ChildOrderAcceptanceId);
