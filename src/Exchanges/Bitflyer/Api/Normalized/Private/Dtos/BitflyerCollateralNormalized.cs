namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerCollateralNormalized(
    decimal Collateral,
    decimal OpenPositionPnl,
    decimal RequireCollateral,
    decimal KeepRate);
