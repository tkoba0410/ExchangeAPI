using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeDepositWithdrawNormalized(
    FreeText Id,
    FreeText Type,
    FreeText Currency,
    decimal Amount,
    FreeText? Address,
    FreeText? TxHash,
    FreeText? State,
    DateTimeOffset? CreatedAt);
