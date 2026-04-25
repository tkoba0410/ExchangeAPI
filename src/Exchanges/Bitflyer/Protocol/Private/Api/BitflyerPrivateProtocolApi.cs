using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.Withdraw;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;

public sealed class BitflyerPrivateProtocolApi : IBitflyerPrivateProtocolApi
{
    private readonly IGetPermissionsProtocolEndpoint _getPermissions;
    private readonly IGetAddressesProtocolEndpoint _getAddresses;
    private readonly IGetCoinInsProtocolEndpoint _getCoinIns;
    private readonly IGetCoinOutsProtocolEndpoint _getCoinOuts;
    private readonly IGetBankAccountsProtocolEndpoint _getBankAccounts;
    private readonly IGetDepositsProtocolEndpoint _getDeposits;
    private readonly IWithdrawProtocolEndpoint _withdraw;
    private readonly IGetWithdrawalsProtocolEndpoint _getWithdrawals;
    private readonly IGetBalanceProtocolEndpoint _getBalance;
    private readonly IGetParentOrdersProtocolEndpoint _getParentOrders;
    private readonly IGetParentOrderProtocolEndpoint _getParentOrder;
    private readonly IGetCollateralProtocolEndpoint _getCollateral;
    private readonly IGetCollateralAccountsProtocolEndpoint _getCollateralAccounts;
    private readonly IGetCollateralHistoryProtocolEndpoint _getCollateralHistory;
    private readonly IGetChildOrdersProtocolEndpoint _getChildOrders;
    private readonly IGetExecutionsProtocolEndpoint _getExecutions;
    private readonly IGetBalanceHistoryProtocolEndpoint _getBalanceHistory;
    private readonly IGetPositionsProtocolEndpoint _getPositions;
    private readonly IGetTradingCommissionProtocolEndpoint _getTradingCommission;
    private readonly ISendChildOrderProtocolEndpoint _sendChildOrder;
    private readonly ISendParentOrderProtocolEndpoint _sendParentOrder;
    private readonly ICancelChildOrderProtocolEndpoint _cancelChildOrder;
    private readonly ICancelAllChildOrdersProtocolEndpoint _cancelAllChildOrders;
    private readonly ICancelParentOrderProtocolEndpoint _cancelParentOrder;

    public BitflyerPrivateProtocolApi(
        IGetPermissionsProtocolEndpoint getPermissions,
        IGetAddressesProtocolEndpoint getAddresses,
        IGetCoinInsProtocolEndpoint getCoinIns,
        IGetCoinOutsProtocolEndpoint getCoinOuts,
        IGetBankAccountsProtocolEndpoint getBankAccounts,
        IGetDepositsProtocolEndpoint getDeposits,
        IWithdrawProtocolEndpoint withdraw,
        IGetWithdrawalsProtocolEndpoint getWithdrawals,
        IGetBalanceProtocolEndpoint getBalance,
        IGetParentOrdersProtocolEndpoint getParentOrders,
        IGetParentOrderProtocolEndpoint getParentOrder,
        IGetCollateralProtocolEndpoint getCollateral,
        IGetCollateralAccountsProtocolEndpoint getCollateralAccounts,
        IGetCollateralHistoryProtocolEndpoint getCollateralHistory,
        IGetChildOrdersProtocolEndpoint getChildOrders,
        IGetExecutionsProtocolEndpoint getExecutions,
        IGetBalanceHistoryProtocolEndpoint getBalanceHistory,
        IGetPositionsProtocolEndpoint getPositions,
        IGetTradingCommissionProtocolEndpoint getTradingCommission,
        ISendChildOrderProtocolEndpoint sendChildOrder,
        ISendParentOrderProtocolEndpoint sendParentOrder,
        ICancelChildOrderProtocolEndpoint cancelChildOrder,
        ICancelAllChildOrdersProtocolEndpoint cancelAllChildOrders,
        ICancelParentOrderProtocolEndpoint cancelParentOrder)
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

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPermissionsAsync(


        CancellationToken cancellationToken = default)


    {


        return _getPermissions.SendAsync(cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPermissionsAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getPermissions.SendAsync(credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetAddressesAsync(


        CancellationToken cancellationToken = default)


    {


        return _getAddresses.SendAsync(cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetAddressesAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getAddresses.SendAsync(credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinInsAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default)


    {


        return _getCoinIns.SendAsync(count, before, after, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinInsAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCoinIns.SendAsync(count, before, after, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinOutsAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default)


    {


        return _getCoinOuts.SendAsync(count, before, after, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinOutsAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCoinOuts.SendAsync(count, before, after, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBankAccountsAsync(


        CancellationToken cancellationToken = default)


    {


        return _getBankAccounts.SendAsync(cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBankAccountsAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getBankAccounts.SendAsync(credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetDepositsAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default)


    {


        return _getDeposits.SendAsync(count, before, after, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetDepositsAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getDeposits.SendAsync(count, before, after, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> WithdrawAsync(


        string bodyJson,


        CancellationToken cancellationToken = default)


    {


        return _withdraw.SendAsync(bodyJson, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> WithdrawAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _withdraw.SendAsync(bodyJson, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetWithdrawalsAsync(


        int? count,


        long? before,


        long? after,


        string? messageId,


        CancellationToken cancellationToken = default)


    {


        return _getWithdrawals.SendAsync(count, before, after, messageId, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetWithdrawalsAsync(


        int? count,


        long? before,


        long? after,


        string? messageId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getWithdrawals.SendAsync(count, before, after, messageId, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceAsync(


        CancellationToken cancellationToken = default)


    {


        return _getBalance.SendAsync(cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getBalance.SendAsync(credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? parentOrderState,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrders.SendAsync(productCode, count, before, after, parentOrderState, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? parentOrderState,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrders.SendAsync(productCode, count, before, after, parentOrderState, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrderAsync(


        string? parentOrderId,


        string? parentOrderAcceptanceId,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrder.SendAsync(parentOrderId, parentOrderAcceptanceId, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrderAsync(


        string? parentOrderId,


        string? parentOrderAcceptanceId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getParentOrder.SendAsync(parentOrderId, parentOrderAcceptanceId, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAsync(


        CancellationToken cancellationToken = default)


    {


        return _getCollateral.SendAsync(cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCollateral.SendAsync(credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAccountsAsync(


        CancellationToken cancellationToken = default)


    {


        return _getCollateralAccounts.SendAsync(cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAccountsAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCollateralAccounts.SendAsync(credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default)


    {


        return _getCollateralHistory.SendAsync(count, before, after, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getCollateralHistory.SendAsync(count, before, after, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetChildOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderState,


        string? childOrderId,


        string? childOrderAcceptanceId,


        string? parentOrderId,


        CancellationToken cancellationToken = default)


    {


        return _getChildOrders.SendAsync(
            productCode,
            count,
            before,
            after,
            childOrderState,
            childOrderId,
            childOrderAcceptanceId,
            parentOrderId,
            cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetChildOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderState,


        string? childOrderId,


        string? childOrderAcceptanceId,


        string? parentOrderId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getChildOrders.SendAsync(productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetExecutionsAsync(


        string productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderId,


        string? childOrderAcceptanceId,


        CancellationToken cancellationToken = default)


    {


        return _getExecutions.SendAsync(
            productCode,
            count,
            before,
            after,
            childOrderId,
            childOrderAcceptanceId,
            cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetExecutionsAsync(


        string productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderId,


        string? childOrderAcceptanceId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getExecutions.SendAsync(productCode, count, before, after, childOrderId, childOrderAcceptanceId, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceHistoryAsync(


        string? currencyCode,


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default)


    {


        return _getBalanceHistory.SendAsync(currencyCode, count, before, after, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceHistoryAsync(


        string? currencyCode,


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getBalanceHistory.SendAsync(currencyCode, count, before, after, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPositionsAsync(


        string productCode,


        CancellationToken cancellationToken = default)


    {


        return _getPositions.SendAsync(productCode, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPositionsAsync(


        string productCode,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getPositions.SendAsync(productCode, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetTradingCommissionAsync(


        string productCode,


        CancellationToken cancellationToken = default)


    {


        return _getTradingCommission.SendAsync(productCode, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetTradingCommissionAsync(


        string productCode,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _getTradingCommission.SendAsync(productCode, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendChildOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default)


    {


        return _sendChildOrder.SendAsync(bodyJson, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendChildOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _sendChildOrder.SendAsync(bodyJson, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendParentOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default)


    {


        return _sendParentOrder.SendAsync(bodyJson, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendParentOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _sendParentOrder.SendAsync(bodyJson, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelChildOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default)


    {


        return _cancelChildOrder.SendAsync(bodyJson, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelChildOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _cancelChildOrder.SendAsync(bodyJson, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersAsync(


        string bodyJson,


        CancellationToken cancellationToken = default)


    {


        return _cancelAllChildOrders.SendAsync(bodyJson, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _cancelAllChildOrders.SendAsync(bodyJson, credentialSession, cancellationToken);


    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelParentOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default)


    {


        return _cancelParentOrder.SendAsync(bodyJson, cancellationToken);


    }



    public Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelParentOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default)


    {


        return _cancelParentOrder.SendAsync(bodyJson, credentialSession, cancellationToken);


    }
}
