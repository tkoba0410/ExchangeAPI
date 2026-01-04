using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed record GetPermissionsRequest;
public sealed record GetBalancesRequest;
public sealed record GetPositionsRequest(RawProductCode ProductCode);
public sealed record GetAccountExecutionsRequest(
    RawProductCode ProductCode,
    string? ChildOrderId = null,
    string? ChildOrderAcceptanceId = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetCollateralRequest;
public sealed record GetCollateralAccountsRequest;
public sealed record GetChildOrdersRequest(
    RawProductCode ProductCode,
    string? ChildOrderStatusState = null,
    string? ChildOrderAcceptanceId = null,
    string? ChildOrderId = null,
    string? ParentOrderId = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetParentOrdersRequest(
    RawProductCode ProductCode,
    string? ParentOrderId = null,
    string? ParentOrderAcceptanceId = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetParentOrderRequest(
    RawProductCode ProductCode,
    string? ParentOrderId = null,
    string? ParentOrderAcceptanceId = null);
public sealed record GetBalanceHistoryRequest(
    string? CurrencyCode = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetTradingCommissionRequest(RawProductCode ProductCode);
public sealed record GetCollateralHistoryRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetAddressesRequest;
public sealed record GetCoinInsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetCoinOutsRequest(string? MessageId = null, int? Count = null, long? Before = null, long? After = null);
public sealed record GetDepositsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetWithdrawalsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetBankAccountsRequest;
