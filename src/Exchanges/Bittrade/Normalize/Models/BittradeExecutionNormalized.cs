using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

internal sealed record BittradeExecutionNormalized(
    string Id,
    string Side,
    decimal Price,
    decimal Size,
    DateTimeOffset Timestamp);
