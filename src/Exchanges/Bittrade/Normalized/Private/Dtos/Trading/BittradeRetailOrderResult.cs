namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;

public sealed record BittradeRetailOrderResult(
    int Code,
    long? OrderId,
    bool? Success,
    string? Message);
