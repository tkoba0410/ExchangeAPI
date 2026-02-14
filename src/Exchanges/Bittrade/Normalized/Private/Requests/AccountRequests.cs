using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

public sealed record GetAccountsRequest;

public sealed record GetAccountsBalanceByAccountIdRequest(AccountId AccountId);

public sealed record GetDepositWithdrawRequest(
    Closed<ExchangeDepositWithdrawType> Type,
    CurrencyCode? Currency = null,
    RequestFrom? From = null,
    RequestSize? Size = null,
    FreeText? Direct = null);

public sealed record GetWithdrawVirtualAddressesRequest;

public sealed record GetRetailAccountBalanceRequest;
