using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;

public sealed record ResolveBitflyerMarketRequest(Symbol Symbol);
