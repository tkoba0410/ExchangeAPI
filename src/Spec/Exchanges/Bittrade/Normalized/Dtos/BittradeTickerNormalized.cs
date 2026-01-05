using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;

public sealed record BittradeTickerNormalized(
    decimal LastTradedPrice,
    DateTimeOffset Timestamp);
