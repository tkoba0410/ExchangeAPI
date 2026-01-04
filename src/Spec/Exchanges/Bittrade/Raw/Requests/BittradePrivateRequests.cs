using System;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Requests;

public sealed record GetAccountsRequest;

public sealed record GetAccountBalanceRequest(string AccountId);

public sealed record GetOpenOrdersRequest(RawSymbol Symbol, string AccountId);

public sealed record GetOrderRequest(RawOrderId OrderId);

public sealed record GetOrderMatchResultsRequest(RawOrderId OrderId);

public sealed record GetOrdersRequest(
    RawSymbol Symbol,
    string States,
    string? StartDate = null,
    string? EndDate = null,
    long? From = null,
    string? Direct = null,
    int? Size = null);

public sealed record GetMatchResultsRequest(
    RawSymbol? Symbol = null,
    string? Types = null,
    string? StartDate = null,
    string? EndDate = null,
    long? From = null,
    string? Direct = null,
    int? Size = null);

public sealed record GetDepositWithdrawsRequest(
    string Type,
    string? Currency = null,
    long? From = null,
    int? Size = null,
    string? Direct = null);

public sealed record GetRetailOrdersRequest(
    int Direct,
    int? Status = null,
    DateTimeOffset? StartTime = null,
    DateTimeOffset? EndTime = null);
