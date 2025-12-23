using System;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;

public sealed record BittradeWireOrder(
    string OrderId,
    string Symbol,
    string Side,
    string Type,
    string? State,
    decimal? Price,
    decimal Size,
    decimal? FilledSize,
    decimal? OutstandingSize,
    DateTimeOffset? CreatedAt);
