
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;

public sealed record GetPermissionsRequest;
public sealed record GetBalancesRequest;
public sealed record GetPositionsRequest(string ProductCode);
public sealed record GetAccountExecutionsRequest(
    string ProductCode,
    string? ChildOrderId = null,
    string? ChildOrderAcceptanceId = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetCollateralRequest;
public sealed record GetCollateralAccountsRequest;
public sealed record GetChildOrdersRequest(
    string ProductCode,
    string? ChildOrderStatusState = null,
    string? ChildOrderAcceptanceId = null,
    string? ChildOrderId = null,
    string? ParentOrderId = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetParentOrdersRequest(
    string ProductCode,
    string? ParentOrderState = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetParentOrderRequest(
    string? ParentOrderId = null,
    string? ParentOrderAcceptanceId = null);
public sealed record GetBalanceHistoryRequest(
    string? CurrencyCode = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetTradingCommissionRequest(string ProductCode);
public sealed record GetCollateralHistoryRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetAddressesRequest;
public sealed record GetCoinInsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetCoinOutsRequest(string? MessageId = null, int? Count = null, long? Before = null, long? After = null);
public sealed record GetDepositsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetWithdrawalsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetBankAccountsRequest;
