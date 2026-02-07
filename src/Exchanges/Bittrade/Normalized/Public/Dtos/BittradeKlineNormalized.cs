using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record BittradeKlineNormalized(
    FreeText OpenTimeUnix,
    decimal Open,
    decimal Close,
    decimal Low,
    decimal High,
    decimal Amount,
    decimal Volume,
    long Count);
