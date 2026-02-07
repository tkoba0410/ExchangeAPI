using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record BittradeSymbolNormalized(
    Symbol Symbol,
    FreeText BaseCurrency,
    FreeText QuoteCurrency,
    int PricePrecision,
    int AmountPrecision,
    decimal MinOrderAmount,
    decimal? MinOrderValue,
    FreeText State);
