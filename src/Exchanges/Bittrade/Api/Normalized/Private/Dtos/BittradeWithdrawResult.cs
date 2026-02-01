namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeWithdrawResult(
    string Status,
    long? WithdrawId);
