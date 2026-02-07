namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record CollateralNormalized(
    decimal Collateral,
    decimal OpenPositionPnl,
    decimal RequireCollateral,
    decimal KeepRate);
