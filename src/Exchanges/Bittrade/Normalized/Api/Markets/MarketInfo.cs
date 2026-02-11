using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Api.Markets;

public sealed record MarketInfo(
    Symbol Symbol,
    ProductCode ProductCode);
