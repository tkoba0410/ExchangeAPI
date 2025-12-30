using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

public sealed record BittradeTickerNormalized(
    decimal LastTradedPrice,
    DateTimeOffset Timestamp);
