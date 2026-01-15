using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;

public sealed record PlaceOrderRequest(OrderRequest Request);

public sealed record CancelOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrderRequest(Symbol Symbol, OrderKey OrderKey);
