using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

internal sealed record BittradeTickerNormalized(
    decimal LastTradedPrice,
    DateTimeOffset Timestamp);
