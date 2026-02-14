using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

public sealed record GetPermissionsRequest;

public sealed record GetBalanceRequest;

public sealed record GetCollateralRequest;

public sealed record GetCollateralAccountsRequest;

public sealed record GetExecutionsPrivateRequest(Symbol Symbol);

public sealed record GetAddressesRequest;

public sealed record GetCoinInsRequest(RequestCount? Count = null, RequestBefore? Before = null, RequestAfter? After = null);

public sealed record GetCoinOutsRequest(
    FreeText? MessageId = null,
    RequestCount? Count = null,
    RequestBefore? Before = null,
    RequestAfter? After = null);

public sealed record GetBankAccountsRequest;

public sealed record GetDepositsRequest(RequestCount? Count = null, RequestBefore? Before = null, RequestAfter? After = null);

public sealed record WithdrawRequest(CurrencyCode CurrencyCode, BankAccountId BankAccountId, WithdrawAmount Amount, FreeText? Code = null);

public sealed record GetWithdrawalsRequest(RequestCount? Count = null, RequestBefore? Before = null, RequestAfter? After = null);

public sealed record GetBalanceHistoryRequest(
    CurrencyCode? CurrencyCode = null,
    RequestCount? Count = null,
    RequestBefore? Before = null,
    RequestAfter? After = null);

public sealed record GetPositionsRequest(Symbol Symbol);

public sealed record GetCollateralHistoryRequest(RequestCount? Count = null, RequestBefore? Before = null, RequestAfter? After = null);

public sealed record GetTradingCommissionRequest(Symbol Symbol);
