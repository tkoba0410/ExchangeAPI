using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;

public sealed record ResolveBitflyerMarketRequest(Symbol Symbol);
