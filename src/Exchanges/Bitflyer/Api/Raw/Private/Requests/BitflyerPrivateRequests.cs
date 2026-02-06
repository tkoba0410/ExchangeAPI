
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;

public sealed record GetPermissionsRequest;
public sealed record GetBalanceRequest;
public sealed record GetPositionsRequest(ProductCode ProductCode);
public sealed record GetExecutionsPrivateRequest(
    ProductCode ProductCode,
    FreeText? ChildOrderId = null,
    FreeText? ChildOrderAcceptanceId = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetCollateralRequest;
public sealed record GetCollateralAccountsRequest;
public sealed record GetChildOrdersRequest(
    ProductCode ProductCode,
    FreeText? ChildOrderStatusState = null,
    FreeText? ChildOrderAcceptanceId = null,
    FreeText? ChildOrderId = null,
    FreeText? ParentOrderId = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetParentOrdersRequest(
    ProductCode ProductCode,
    FreeText? ParentOrderState = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetParentOrderRequest(
    FreeText? ParentOrderId = null,
    FreeText? ParentOrderAcceptanceId = null);
public sealed record GetBalanceHistoryRequest(
    CurrencyCode? CurrencyCode = null,
    int? Count = null,
    long? Before = null,
    long? After = null);
public sealed record GetTradingCommissionRequest(ProductCode ProductCode);
public sealed record GetCollateralHistoryRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetAddressesRequest;
public sealed record GetCoinInsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetCoinOutsRequest(FreeText? MessageId = null, int? Count = null, long? Before = null, long? After = null);
public sealed record GetDepositsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetWithdrawalsRequest(int? Count = null, long? Before = null, long? After = null);
public sealed record GetBankAccountsRequest;
