namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

internal sealed record BittradeOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
