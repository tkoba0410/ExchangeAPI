namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerTradingCommissionNormalized(
    string ProductCode,
    decimal? CommissionRate);
