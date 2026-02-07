using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerHealthNormalized(FreeText? Status);
