using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Requests;

public sealed record PlaceOrderRequest(OrderRequest Request);

public sealed record CancelOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetAccountExecutionsRequest(Symbol Symbol, int? Limit = null);
