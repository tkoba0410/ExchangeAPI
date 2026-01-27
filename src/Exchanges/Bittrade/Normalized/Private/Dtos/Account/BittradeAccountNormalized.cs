namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Account;

public sealed record BittradeAccountNormalized(
    string Id,
    string Type,
    string? SubType,
    string State);
