using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record GetAccountsRequest;

public sealed record GetAccountsBalanceByAccountIdRequest(AccountId AccountId);

public sealed record GetOpenOrdersRequest(Symbol Symbol, AccountId AccountId);

public sealed record GetOrdersRequest;

public sealed record GetOrdersByOrderIdRequest(OrderId OrderId);

public sealed record GetOrdersMatchResultsByOrderIdRequest(OrderId OrderId);

public sealed record GetMatchResultsRequest(
    Symbol? Symbol = null,
    FreeText? Types = null,
    FreeText? StartDate = null,
    FreeText? EndDate = null,
    long? From = null,
    FreeText? Direct = null,
    int? Size = null);

public sealed record GetDepositWithdrawRequest(
    FreeText Type,
    FreeText? Currency = null,
    long? From = null,
    int? Size = null,
    FreeText? Direct = null);

public sealed record GetWithdrawVirtualAddressesRequest;

public sealed record GetRetailOrderListRequest(
    int Direct,
    int? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null);

public sealed record GetRetailOrderDetailByOrderIdRequest(OrderId OrderId);

public sealed record GetRetailAccountBalanceRequest;
