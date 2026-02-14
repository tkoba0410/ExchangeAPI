using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record TradingCommissionNormalized(
    ProductCode ProductCode,
    decimal? CommissionRate);
