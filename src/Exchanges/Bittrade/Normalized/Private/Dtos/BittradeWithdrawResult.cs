namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeWithdrawResult(
    string Status,
    long? WithdrawId);
