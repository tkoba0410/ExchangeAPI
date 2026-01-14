using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Requests;

public sealed record PlaceOrderRequest(OrderRequest Request);

public sealed record CancelOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetAccountExecutionsRequest(Symbol Symbol, int? Limit = null);
