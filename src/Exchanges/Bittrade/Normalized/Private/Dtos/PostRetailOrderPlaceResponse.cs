using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record PostRetailOrderPlaceResponse(
    int Code,
    long? OrderId,
    bool? Success,
    FreeText? Message);
