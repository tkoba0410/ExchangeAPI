using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

public sealed record PlaceOrderRequest(BitflyerOrderRequest Request);

public sealed record CancelOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrderRequest(Symbol Symbol, OrderKey OrderKey);
