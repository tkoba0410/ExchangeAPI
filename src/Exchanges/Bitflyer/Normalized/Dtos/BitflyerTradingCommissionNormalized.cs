namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;

public sealed record BitflyerTradingCommissionNormalized(
    string ProductCode,
    decimal? CommissionRate);
