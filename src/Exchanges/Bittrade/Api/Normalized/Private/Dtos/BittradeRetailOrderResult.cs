using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeRetailOrderResult(
    int Code,
    long? OrderId,
    bool? Success,
    FreeText? Message);
