namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record BitflyerTradingCommissionNormalized(
    string ProductCode,
    decimal? CommissionRate);
