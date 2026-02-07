using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeAccountNormalized(
    FreeText Id,
    FreeText Type,
    FreeText? SubType,
    FreeText State);
