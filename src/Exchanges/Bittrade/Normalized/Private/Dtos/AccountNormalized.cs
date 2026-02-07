using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record AccountNormalized(
    AccountId AccountId,
    Closed<ExchangeAccountType> Type,
    Closed<ExchangeAccountSubType>? SubType,
    Closed<ExchangeAccountState> State);
