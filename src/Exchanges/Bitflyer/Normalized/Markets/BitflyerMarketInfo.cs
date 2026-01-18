using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;

public sealed record BitflyerMarketInfo(
    Symbol Symbol,
    string ProductCode);
