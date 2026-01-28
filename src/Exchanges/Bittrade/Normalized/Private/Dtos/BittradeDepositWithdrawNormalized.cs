using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeDepositWithdrawNormalized(
    string Id,
    string Type,
    string Currency,
    decimal Amount,
    string? Address,
    string? TxHash,
    string? State,
    DateTimeOffset? CreatedAt);
