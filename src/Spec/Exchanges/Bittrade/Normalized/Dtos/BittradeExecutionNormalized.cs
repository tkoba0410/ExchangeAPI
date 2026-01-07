using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;

public sealed record BittradeExecutionNormalized(
    string Id,
    string Side,
    decimal Price,
    decimal Size,
    DateTimeOffset Timestamp);
