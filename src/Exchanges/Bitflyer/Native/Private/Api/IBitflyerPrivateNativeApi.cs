using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;

public interface IBitflyerPrivateNativeApi
{
    Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        WithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> GetExecutionsCallAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelChildOrderRequest, Unit>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRequest, Unit>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);
}
