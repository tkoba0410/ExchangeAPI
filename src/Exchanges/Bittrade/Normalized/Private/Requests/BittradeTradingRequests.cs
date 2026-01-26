using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

public sealed record PlaceOrderRequest(BittradeOrderRequest Request);

public sealed record GetOrdersRequest;

public sealed record CancelOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetAccountExecutionsRequest(Symbol Symbol, int? Limit = null);

public sealed record GetRetailOrderListRequest(
    int Direct,
    int? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null);

public sealed record GetRetailOrderDetailByOrderIdRequest(string OrderId);

public sealed record PostRetailOrderHistoryRequest(
    Symbol? Symbol = null,
    int? Direct = null,
    int? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null,
    int? Size = null);

public sealed record PostRetailOrderDetailRequest(string OrderId);

public sealed record BittradeRetailOrderRequest(
    Symbol Symbol,
    int Type,
    decimal? Price = null,
    decimal? Amount = null,
    decimal? CashAmount = null);

public sealed record PostRetailOrderCreateRequest(BittradeRetailOrderRequest Request);

public sealed record PostRetailOrderCancelByOrderIdRequest(string OrderId);

public sealed record PostWithdrawVirtualByAddressIdCreateRequest(string AddressId);

public sealed record PostWithdrawVirtualByWithdrawIdPlaceRequest(string WithdrawId);

public sealed record PostWithdrawVirtualByWithdrawIdCancelRequest(string WithdrawId);
