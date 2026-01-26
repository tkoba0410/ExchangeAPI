namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;

public sealed record BittradeWithdrawResult(
    string Status,
    long? WithdrawId);
