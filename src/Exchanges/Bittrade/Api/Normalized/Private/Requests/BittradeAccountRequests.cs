namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;

public sealed record GetAccountsRequest;

public sealed record GetBalancesRequest(string AccountId);

public sealed record GetDepositWithdrawRequest(
    string Type,
    string? Currency = null,
    long? From = null,
    int? Size = null,
    string? Direct = null);

public sealed record GetWithdrawVirtualAddressesRequest;

public sealed record GetRetailAccountBalanceRequest;
