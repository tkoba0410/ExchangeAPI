namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeAccountNormalized(
    string Id,
    string Type,
    string? SubType,
    string State);
