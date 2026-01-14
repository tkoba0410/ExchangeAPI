namespace ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;

public sealed record BittradeSymbolNormalized(
    string Symbol,
    string BaseCurrency,
    string QuoteCurrency,
    int PricePrecision,
    int AmountPrecision,
    decimal MinOrderAmount,
    decimal? MinOrderValue,
    string State);
