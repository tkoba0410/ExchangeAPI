using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record PostWithdrawVirtualByWithdrawIdPlaceResponse(
    FreeText Status,
    long? WithdrawId);
