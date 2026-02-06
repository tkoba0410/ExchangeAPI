using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests;

public sealed record GetPermissionsRequest;

public sealed record GetBalanceRequest;

public sealed record GetCollateralRequest;

public sealed record GetCollateralAccountsRequest;

public sealed record GetExecutionsPrivateRequest(Symbol Symbol);

public sealed record GetAddressesRequest;

public sealed record GetCoinInsRequest(int? Count = null, long? Before = null, long? After = null);

public sealed record GetCoinOutsRequest(FreeText? MessageId = null, int? Count = null, long? Before = null, long? After = null);

public sealed record GetBankAccountsRequest;

public sealed record GetDepositsRequest(int? Count = null, long? Before = null, long? After = null);

public sealed record WithdrawRequest(CurrencyCode CurrencyCode, int BankAccountId, decimal Amount, FreeText? Code = null);

public sealed record GetWithdrawalsRequest(int? Count = null, long? Before = null, long? After = null);

public sealed record GetBalanceHistoryRequest(CurrencyCode? CurrencyCode = null, int? Count = null, long? Before = null, long? After = null);

public sealed record GetPositionsRequest(Symbol Symbol);

public sealed record GetCollateralHistoryRequest(int? Count = null, long? Before = null, long? After = null);

public sealed record GetTradingCommissionRequest(Symbol Symbol);