using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record SendChildOrderResponse(
    OrderKey Key,
    ExchangeOrderId? ExchangeOrderId = null,
    AcceptanceId? AcceptanceId = null);
public sealed record CancelChildOrderResponse(bool IsSuccess);
public sealed record CancelAllChildOrdersResponse(bool IsSuccess);

public sealed record GetChildOrdersItem(OpenOrder Value);
public sealed record GetChildOrdersResponse(IReadOnlyList<GetChildOrdersItem> Items);

public sealed record SendParentOrderResponse(ParentOrderAcceptance Item);
public sealed record CancelParentOrderResponse(ParentOrderCancelResult Item);

public sealed record GetParentOrdersItem(ParentOrderNormalized Value);
public sealed record GetParentOrdersResponse(IReadOnlyList<GetParentOrdersItem> Items);
public sealed record GetParentOrderResponse(ParentOrderDetailNormalized Item);

public sealed record GetBalanceItem(BalanceEntryNormalized Value);
public sealed record GetBalanceResponse(IReadOnlyList<GetBalanceItem> Items);

public sealed record GetPermissionsItem(FreeText Value);
public sealed record GetPermissionsResponse(IReadOnlyList<GetPermissionsItem> Items);

public sealed record GetCollateralResponse(CollateralNormalized Item);

public sealed record GetCollateralAccountsItem(CollateralAccountNormalized Value);
public sealed record GetCollateralAccountsResponse(IReadOnlyList<GetCollateralAccountsItem> Items);

public sealed record GetAddressesResponse(FreeText RawJson);
public sealed record GetCoinInsResponse(FreeText RawJson);
public sealed record GetCoinOutsResponse(FreeText RawJson);
public sealed record GetBankAccountsResponse(FreeText RawJson);
public sealed record GetDepositsResponse(FreeText RawJson);

public sealed record WithdrawResponse(WithdrawResult Item);
public sealed record GetWithdrawalsResponse(FreeText RawJson);

public sealed record GetExecutionsPrivateItem(ExecutionAccountNormalized Value);
public sealed record GetExecutionsPrivateResponse(IReadOnlyList<GetExecutionsPrivateItem> Items);

public sealed record GetBalanceHistoryResponse(FreeText RawJson);

public sealed record GetPositionsItem(PositionNormalized Value);
public sealed record GetPositionsResponse(IReadOnlyList<GetPositionsItem> Items);

public sealed record GetCollateralHistoryResponse(FreeText RawJson);
public sealed record GetTradingCommissionResponse(TradingCommissionNormalized Item);
