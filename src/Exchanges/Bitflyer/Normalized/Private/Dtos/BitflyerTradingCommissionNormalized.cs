using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record BitflyerTradingCommissionNormalized(
    ProductCode ProductCode,
    decimal? CommissionRate);
