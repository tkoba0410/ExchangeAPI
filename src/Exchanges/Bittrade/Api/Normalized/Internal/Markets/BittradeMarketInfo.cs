using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;

public sealed record BittradeMarketInfo(
    Symbol Symbol,
    ProductCode ProductCode);
