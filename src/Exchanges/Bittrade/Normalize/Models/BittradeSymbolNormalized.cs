namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

public sealed record BittradeSymbolNormalized(
    string Symbol,
    string BaseCurrency,
    string QuoteCurrency,
    int PricePrecision,
    int AmountPrecision,
    decimal MinOrderAmount,
    decimal? MinOrderValue,
    string State);
