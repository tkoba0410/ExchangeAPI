using System;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerTickerNormalized(
    string ProductCode,
    decimal LastTradedPrice,
    DateTimeOffset Timestamp);
