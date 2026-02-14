using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record DepositWithdrawNormalized(
    TransactionId TransactionId,
    Closed<ExchangeDepositWithdrawType> Type,
    CurrencyCode Currency,
    decimal Amount,
    FreeText? Address,
    FreeText? TxHash,
    Closed<ExchangeDepositWithdrawState>? State,
    DateTimeOffset? CreatedAt);
