using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
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

    public Task<CallResult<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(


        GetPermissionsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getPermissions.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(


        GetPermissionsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getPermissions.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> GetAddressesAsync(


        GetAddressesRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getAddresses.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> GetAddressesAsync(


        GetAddressesRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getAddresses.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> GetCoinInsAsync(


        GetCoinInsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getCoinIns.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> GetCoinInsAsync(


        GetCoinInsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCoinIns.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> GetCoinOutsAsync(


        GetCoinOutsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getCoinOuts.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> GetCoinOutsAsync(


        GetCoinOutsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCoinOuts.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> GetBankAccountsAsync(


        GetBankAccountsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getBankAccounts.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> GetBankAccountsAsync(


        GetBankAccountsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getBankAccounts.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> GetDepositsAsync(


        GetDepositsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getDeposits.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> GetDepositsAsync(


        GetDepositsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getDeposits.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<WithdrawRequest, WithdrawResponse>> WithdrawAsync(


        WithdrawRequest request,


        CancellationToken cancellationToken = default)


    {


        return _withdraw.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<WithdrawRequest, WithdrawResponse>> WithdrawAsync(


        WithdrawRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _withdraw.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> GetWithdrawalsAsync(


        GetWithdrawalsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getWithdrawals.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> GetWithdrawalsAsync(


        GetWithdrawalsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getWithdrawals.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(


        GetBalanceRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getBalance.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(


        GetBalanceRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getBalance.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> GetParentOrdersAsync(


        GetParentOrdersRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrders.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> GetParentOrdersAsync(


        GetParentOrdersRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrders.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderAsync(


        GetParentOrderRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrder.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderAsync(


        GetParentOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrder.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(


        GetCollateralRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getCollateral.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(


        GetCollateralRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCollateral.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsAsync(


        GetCollateralAccountsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getCollateralAccounts.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsAsync(


        GetCollateralAccountsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCollateralAccounts.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryAsync(


        GetCollateralHistoryRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getCollateralHistory.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryAsync(


        GetCollateralHistoryRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCollateralHistory.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersAsync(


        GetChildOrdersRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getChildOrders.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersAsync(


        GetChildOrdersRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getChildOrders.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> GetExecutionsAsync(


        GetExecutionsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getExecutions.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> GetExecutionsAsync(


        GetExecutionsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getExecutions.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryAsync(


        GetBalanceHistoryRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getBalanceHistory.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryAsync(


        GetBalanceHistoryRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getBalanceHistory.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(


        GetPositionsRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getPositions.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(


        GetPositionsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getPositions.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionAsync(


        GetTradingCommissionRequest request,


        CancellationToken cancellationToken = default)


    {


        return _getTradingCommission.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionAsync(


        GetTradingCommissionRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getTradingCommission.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderAsync(


        SendChildOrderRequest request,


        CancellationToken cancellationToken = default)


    {


        return _sendChildOrder.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderAsync(


        SendChildOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _sendChildOrder.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderAsync(


        SendParentOrderRequest request,


        CancellationToken cancellationToken = default)


    {


        return _sendParentOrder.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderAsync(


        SendParentOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _sendParentOrder.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<CancelChildOrderRequest, Unit>> CancelChildOrderAsync(


        CancelChildOrderRequest request,


        CancellationToken cancellationToken = default)


    {


        return _cancelChildOrder.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<CancelChildOrderRequest, Unit>> CancelChildOrderAsync(


        CancelChildOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _cancelChildOrder.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersAsync(


        CancelAllChildOrdersRequest request,


        CancellationToken cancellationToken = default)


    {


        return _cancelAllChildOrders.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersAsync(


        CancelAllChildOrdersRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _cancelAllChildOrders.CallAsync(request, credentialSession, cancellationToken);


    }

    public Task<CallResult<CancelParentOrderRequest, Unit>> CancelParentOrderAsync(


        CancelParentOrderRequest request,


        CancellationToken cancellationToken = default)


    {


        return _cancelParentOrder.CallAsync(request, cancellationToken);


    }



    public Task<CallResult<CancelParentOrderRequest, Unit>> CancelParentOrderAsync(


        CancelParentOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _cancelParentOrder.CallAsync(request, credentialSession, cancellationToken);


    }
}
