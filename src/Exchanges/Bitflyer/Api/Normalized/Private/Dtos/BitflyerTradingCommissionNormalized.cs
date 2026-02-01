namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerTradingCommissionNormalized(
    string ProductCode,
    decimal? CommissionRate);
