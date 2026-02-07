using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

public sealed record PostOrdersPlaceRequest(OrderRequest Request);

public sealed record GetOrdersRequest;

public sealed record PostOrdersSubmitCancelByOrderIdRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record PostOrdersBatchCancelRequest(IReadOnlyList<OrderId> OrderIds);

public sealed record PostOrdersBatchCancelOpenOrdersRequest(
    Symbol? Symbol = null,
    Side? Side = null,
    decimal? Size = null,
    decimal? Price = null,
    DateTimeOffset? CreatedAt = null);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrdersByOrderIdRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOrdersMatchResultsByOrderIdRequest(OrderKey OrderKey);

public sealed record GetMatchResultsRequest(Symbol Symbol, int? Limit = null);

public sealed record GetRetailOrderListRequest(
    int Direct,
    int? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null);

public sealed record GetRetailOrderDetailByOrderIdRequest(OrderId OrderId);

public sealed record PostRetailOrderHistoryRequest(
    Symbol? Symbol = null,
    int? Direct = null,
    int? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null,
    int? Size = null);

public sealed record PostRetailOrderDetailRequest(OrderId OrderId);

public sealed record RetailOrderRequest(
    Symbol Symbol,
    int Type,
    decimal? Price = null,
    decimal? Amount = null,
    decimal? CashAmount = null);

public sealed record PostRetailOrderCreateRequest(RetailOrderRequest Request);

public sealed record PostRetailOrderPlaceRequest(RetailOrderRequest Request);

public sealed record PostRetailOrderCancelByOrderIdRequest(OrderId OrderId);

public sealed record PostWithdrawApiCreateRequest(
    FreeText Address,
    decimal Amount,
    FreeText Currency,
    decimal? Fee = null,
    FreeText? AddressTag = null);

public sealed record PostWithdrawVirtualByAddressIdCreateRequest(FreeText AddressId);

public sealed record PostWithdrawVirtualByWithdrawIdPlaceRequest(FreeText WithdrawId);

public sealed record PostWithdrawVirtualByWithdrawIdCancelRequest(FreeText WithdrawId);
