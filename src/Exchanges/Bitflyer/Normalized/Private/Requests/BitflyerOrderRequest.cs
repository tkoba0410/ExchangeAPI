using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

public sealed record BitflyerOrderRequest(
    Symbol Symbol,
    Side Side,
    OrderType OrderType,
    Size Size,
    Price? Price = null);
