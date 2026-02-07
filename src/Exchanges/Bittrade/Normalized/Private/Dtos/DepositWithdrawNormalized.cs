using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record DepositWithdrawNormalized(
    FreeText Id,
    FreeText Type,
    FreeText Currency,
    decimal Amount,
    FreeText? Address,
    FreeText? TxHash,
    FreeText? State,
    DateTimeOffset? CreatedAt);
