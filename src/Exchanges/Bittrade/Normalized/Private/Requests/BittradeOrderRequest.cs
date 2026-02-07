using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

public sealed record BittradeOrderRequest(
    Symbol Symbol,
    Side Side,
    OrderType OrderType,
    Size Size,
    Price? Price = null);
