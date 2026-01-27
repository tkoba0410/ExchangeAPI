using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Markets;

public sealed record BittradeMarketInfo(
    Symbol Symbol,
    string ProductCode);
