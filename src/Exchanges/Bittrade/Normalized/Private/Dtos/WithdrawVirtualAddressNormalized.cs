using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record WithdrawVirtualAddressNormalized(
    long? AddressId,
    FreeText? Currency,
    FreeText? Address,
    FreeText? AddressTag,
    FreeText? Chain,
    FreeText? Note,
    FreeText? State,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
