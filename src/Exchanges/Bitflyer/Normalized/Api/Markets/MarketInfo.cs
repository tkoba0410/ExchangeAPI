using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Api.Markets;

public sealed record MarketInfo(
    Symbol Symbol,
    ProductCode ProductCode);
