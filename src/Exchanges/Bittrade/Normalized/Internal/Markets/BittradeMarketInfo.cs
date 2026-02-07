using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;

public sealed record BittradeMarketInfo(
    Symbol Symbol,
    ProductCode ProductCode);
