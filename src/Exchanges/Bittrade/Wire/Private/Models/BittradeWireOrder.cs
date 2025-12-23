using System;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;

public sealed record BittradeWireOrder(
    string OrderId,
    string Symbol,
    string Side,
    string Type,
    decimal? Price,
    decimal Size,
    DateTimeOffset? CreatedAt);
