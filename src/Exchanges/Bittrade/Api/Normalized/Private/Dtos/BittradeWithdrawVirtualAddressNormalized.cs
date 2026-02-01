using System;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeWithdrawVirtualAddressNormalized(
    long? AddressId,
    string? Currency,
    string? Address,
    string? AddressTag,
    string? Chain,
    string? Note,
    string? State,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
