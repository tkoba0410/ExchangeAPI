namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public.Models;

internal sealed record BittradeWireTicker(
    decimal BestBid,
    decimal BestAsk,
    decimal Last,
    decimal Volume,
    DateTimeOffset Timestamp
);
