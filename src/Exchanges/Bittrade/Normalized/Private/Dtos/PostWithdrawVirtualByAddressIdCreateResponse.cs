using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record PostWithdrawVirtualByAddressIdCreateResponse(
    FreeText Status,
    long? WithdrawId);
