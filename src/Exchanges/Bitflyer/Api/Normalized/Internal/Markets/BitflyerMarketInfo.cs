using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Markets;

public sealed record BitflyerMarketInfo(
    Symbol Symbol,
    string ProductCode);
