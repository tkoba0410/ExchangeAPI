using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

public sealed record PostOrdersPlaceRequest(OrderRequest Request);

public sealed record GetOrdersRequest;

public sealed record PostOrdersSubmitCancelByOrderIdRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record PostOrdersBatchCancelRequest(IReadOnlyList<OrderId> OrderIds);

public sealed record PostOrdersBatchCancelOpenOrdersRequest(
    Symbol? Symbol = null,
    Side? Side = null,
    Size? Size = null,
    decimal? Price = null,
    DateTimeOffset? CreatedAt = null);

public sealed record GetOpenOrdersRequest(Symbol Symbol);

public sealed record GetOrdersByOrderIdRequest(Symbol Symbol, OrderKey OrderKey);

public sealed record GetOrdersMatchResultsByOrderIdRequest(OrderKey OrderKey);

public sealed record GetMatchResultsRequest(Symbol Symbol, RequestSize? Limit = null);

public sealed record GetRetailOrderListRequest(
    RetailOrderDirection Direct,
    RetailOrderStatus? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null);

public sealed record GetRetailOrderDetailByOrderIdRequest(OrderId OrderId);

public sealed record PostRetailOrderHistoryRequest(
    Symbol? Symbol = null,
    RetailOrderDirection? Direct = null,
    RetailOrderStatus? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null,
    RequestSize? Size = null);

public sealed record PostRetailOrderDetailRequest(OrderId OrderId);

public sealed record RetailOrderRequest(
    Symbol Symbol,
    RetailOrderType Type,
    decimal? Price = null,
    RetailOrderAmount? Amount = null,
    RetailOrderAmount? CashAmount = null);

public sealed record PostRetailOrderCreateRequest(RetailOrderRequest Request);

public sealed record PostRetailOrderPlaceRequest(RetailOrderRequest Request);

public sealed record PostRetailOrderCancelByOrderIdRequest(OrderId OrderId);

public sealed record PostWithdrawApiCreateRequest(
    FreeText Address,
    WithdrawAmount Amount,
    FreeText Currency,
    WithdrawFee? Fee = null,
    FreeText? AddressTag = null);

public sealed record PostWithdrawVirtualByAddressIdCreateRequest(FreeText AddressId);

public sealed record PostWithdrawVirtualByWithdrawIdPlaceRequest(FreeText WithdrawId);

public sealed record PostWithdrawVirtualByWithdrawIdCancelRequest(FreeText WithdrawId);
