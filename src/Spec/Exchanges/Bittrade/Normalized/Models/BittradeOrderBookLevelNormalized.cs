namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

public sealed record BittradeOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
