using System;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;

public sealed record BittradeWireOpenOrder(
    string RawOrderId,
    string RawSymbol,
    string Side,
    string Type,
    string State,
    decimal? Price,
    decimal Size,
    decimal FilledSize,
    DateTimeOffset CreatedAt);
