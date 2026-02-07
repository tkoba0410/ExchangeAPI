using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record RetailOrderResult(
    int Code,
    long? OrderId,
    bool? Success,
    FreeText? Message);
