using System;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Models;

internal sealed record BittradeWireOrder(
    string RawOrderId,
    string RawSymbol,
    string Side,
    string Type,
    string? State,
    decimal? Price,
    decimal Size,
    decimal? FilledSize,
    decimal? OutstandingSize,
    DateTimeOffset? CreatedAt);
