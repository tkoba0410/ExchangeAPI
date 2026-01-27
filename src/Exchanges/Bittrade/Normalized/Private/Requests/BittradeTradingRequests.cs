using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

public sealed record PostOrdersPlaceRequest(BittradeOrderRequest Request);

public sealed record GetOrdersRequest;

public sealed record PostOrdersSubmitCancelByOrderIdRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record PostOrdersBatchCancelRequest(IReadOnlyList<string> OrderIds);

public sealed record PostOrdersBatchCancelOpenOrdersRequest(
    Symbol? Symbol = null,
    Side? Side = null,
    decimal? Size = null,
    decimal? Price = null,
    DateTimeOffset? CreatedAt = null);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrderRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOrdersMatchResultsByOrderIdRequest(OrderKey OrderKey);

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

public sealed record PostRetailOrderPlaceRequest(BittradeRetailOrderRequest Request);

public sealed record PostRetailOrderCancelByOrderIdRequest(string OrderId);

public sealed record PostWithdrawApiCreateRequest(
    string Address,
    decimal Amount,
    string Currency,
    decimal? Fee = null,
    string? AddressTag = null);

public sealed record PostWithdrawVirtualByAddressIdCreateRequest(string AddressId);

public sealed record PostWithdrawVirtualByWithdrawIdPlaceRequest(string WithdrawId);

public sealed record PostWithdrawVirtualByWithdrawIdCancelRequest(string WithdrawId);
