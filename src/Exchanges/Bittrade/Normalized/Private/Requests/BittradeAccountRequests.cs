using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

public sealed record GetAccountsRequest;

public sealed record GetAccountsBalanceByAccountIdRequest(FreeText AccountId);

public sealed record GetDepositWithdrawRequest(
    FreeText Type,
    FreeText? Currency = null,
    long? From = null,
    int? Size = null,
    FreeText? Direct = null);

public sealed record GetWithdrawVirtualAddressesRequest;

public sealed record GetRetailAccountBalanceRequest;
