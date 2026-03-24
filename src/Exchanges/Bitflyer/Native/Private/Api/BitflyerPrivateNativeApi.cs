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

public sealed class BitflyerPrivateNativeApi : IBitflyerPrivateNativeApi
{
    private readonly IGetPermissionsNativeEndpoint _getPermissions;
    private readonly IGetAddressesNativeEndpoint _getAddresses;
    private readonly IGetCoinInsNativeEndpoint _getCoinIns;
    private readonly IGetCoinOutsNativeEndpoint _getCoinOuts;
    private readonly IGetBankAccountsNativeEndpoint _getBankAccounts;
    private readonly IGetDepositsNativeEndpoint _getDeposits;
    private readonly IWithdrawNativeEndpoint _withdraw;
    private readonly IGetWithdrawalsNativeEndpoint _getWithdrawals;
    private readonly IGetBalanceNativeEndpoint _getBalance;
    private readonly IGetParentOrdersNativeEndpoint _getParentOrders;
    private readonly IGetParentOrderNativeEndpoint _getParentOrder;
    private readonly IGetCollateralNativeEndpoint _getCollateral;
    private readonly IGetCollateralAccountsNativeEndpoint _getCollateralAccounts;
    private readonly IGetCollateralHistoryNativeEndpoint _getCollateralHistory;
    private readonly IGetChildOrdersNativeEndpoint _getChildOrders;
    private readonly IGetExecutionsNativeEndpoint _getExecutions;
    private readonly IGetBalanceHistoryNativeEndpoint _getBalanceHistory;
    private readonly IGetPositionsNativeEndpoint _getPositions;
    private readonly IGetTradingCommissionNativeEndpoint _getTradingCommission;
    private readonly ISendChildOrderNativeEndpoint _sendChildOrder;
    private readonly ISendParentOrderNativeEndpoint _sendParentOrder;
    private readonly ICancelChildOrderNativeEndpoint _cancelChildOrder;
    private readonly ICancelAllChildOrdersNativeEndpoint _cancelAllChildOrders;
    private readonly ICancelParentOrderNativeEndpoint _cancelParentOrder;

    public BitflyerPrivateNativeApi(
        IGetPermissionsNativeEndpoint getPermissions,
        IGetAddressesNativeEndpoint getAddresses,
        IGetCoinInsNativeEndpoint getCoinIns,
        IGetCoinOutsNativeEndpoint getCoinOuts,
        IGetBankAccountsNativeEndpoint getBankAccounts,
        IGetDepositsNativeEndpoint getDeposits,
        IWithdrawNativeEndpoint withdraw,
        IGetWithdrawalsNativeEndpoint getWithdrawals,
        IGetBalanceNativeEndpoint getBalance,
        IGetParentOrdersNativeEndpoint getParentOrders,
        IGetParentOrderNativeEndpoint getParentOrder,
        IGetCollateralNativeEndpoint getCollateral,
        IGetCollateralAccountsNativeEndpoint getCollateralAccounts,
        IGetCollateralHistoryNativeEndpoint getCollateralHistory,
        IGetChildOrdersNativeEndpoint getChildOrders,
        IGetExecutionsNativeEndpoint getExecutions,
        IGetBalanceHistoryNativeEndpoint getBalanceHistory,
        IGetPositionsNativeEndpoint getPositions,
        IGetTradingCommissionNativeEndpoint getTradingCommission,
        ISendChildOrderNativeEndpoint sendChildOrder,
        ISendParentOrderNativeEndpoint sendParentOrder,
        ICancelChildOrderNativeEndpoint cancelChildOrder,
        ICancelAllChildOrdersNativeEndpoint cancelAllChildOrders,
        ICancelParentOrderNativeEndpoint cancelParentOrder)
    {
        _getPermissions = getPermissions;
        _getAddresses = getAddresses;
        _getCoinIns = getCoinIns;
        _getCoinOuts = getCoinOuts;
        _getBankAccounts = getBankAccounts;
        _getDeposits = getDeposits;
        _withdraw = withdraw;
        _getWithdrawals = getWithdrawals;
        _getBalance = getBalance;
        _getParentOrders = getParentOrders;
        _getParentOrder = getParentOrder;
        _getCollateral = getCollateral;
        _getCollateralAccounts = getCollateralAccounts;
        _getCollateralHistory = getCollateralHistory;
        _getChildOrders = getChildOrders;
        _getExecutions = getExecutions;
        _getBalanceHistory = getBalanceHistory;
        _getPositions = getPositions;
        _getTradingCommission = getTradingCommission;
        _sendChildOrder = sendChildOrder;
        _sendParentOrder = sendParentOrder;
        _cancelChildOrder = cancelChildOrder;
        _cancelAllChildOrders = cancelAllChildOrders;
        _cancelParentOrder = cancelParentOrder;
    }

    public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getPermissions.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getAddresses.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getCoinIns.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getCoinOuts.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBankAccounts.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getDeposits.CallAsync(request, cancellationToken);
    }

    public Task<Call<WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        WithdrawRequest request,
        CancellationToken cancellationToken = default)
    {
        return _withdraw.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getWithdrawals.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBalance.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getParentOrders.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getParentOrder.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getCollateral.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getCollateralAccounts.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getCollateralHistory.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getChildOrders.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> GetExecutionsCallAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getExecutions.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBalanceHistory.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getPositions.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getTradingCommission.CallAsync(request, cancellationToken);
    }

    public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _sendChildOrder.CallAsync(request, cancellationToken);
    }

    public Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _sendParentOrder.CallAsync(request, cancellationToken);
    }

    public Task<Call<CancelChildOrderRequest, Unit>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _cancelChildOrder.CallAsync(request, cancellationToken);
    }

    public Task<Call<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        return _cancelAllChildOrders.CallAsync(request, cancellationToken);
    }

    public Task<Call<CancelParentOrderRequest, Unit>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _cancelParentOrder.CallAsync(request, cancellationToken);
    }
}
