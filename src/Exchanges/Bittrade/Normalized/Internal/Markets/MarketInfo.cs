using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;

public sealed record MarketInfo(
    Symbol Symbol,
    ProductCode ProductCode);
