using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

public sealed record SendChildOrderRequest(OrderRequest Request);

public sealed record CancelChildOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetChildOrdersRequest(Symbol Symbol);

public sealed record CancelAllChildOrdersRequest(Symbol Symbol);
