using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;

public sealed record BittradeSymbolNormalized(
    FreeText Symbol,
    FreeText BaseCurrency,
    FreeText QuoteCurrency,
    int PricePrecision,
    int AmountPrecision,
    decimal MinOrderAmount,
    decimal? MinOrderValue,
    FreeText State);
