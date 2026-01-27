namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;

public sealed record BitflyerCollateralNormalized(
    decimal Collateral,
    decimal OpenPositionPnl,
    decimal RequireCollateral,
    decimal KeepRate);
