namespace ExchangeApi.Exchanges.Bittrade.Wire.Public.Models;

public sealed record BittradeWireTicker(
    decimal BestBid,
    decimal BestAsk,
    decimal Last,
    decimal Volume,
    DateTimeOffset Timestamp
);
