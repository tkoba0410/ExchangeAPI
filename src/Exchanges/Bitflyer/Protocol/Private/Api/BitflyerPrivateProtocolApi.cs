using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.Withdraw;
using ExchangeApi.Primitives.Calls;
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

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getPermissions.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getAddresses.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetCoinInsCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return _getCoinIns.SendAsync(count, before, after, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetCoinOutsCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return _getCoinOuts.SendAsync(count, before, after, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getBankAccounts.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetDepositsCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return _getDeposits.SendAsync(count, before, after, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> WithdrawCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _withdraw.SendAsync(bodyJson, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetWithdrawalsCallAsync(
        int? count,
        long? before,
        long? after,
        string? messageId,
        CancellationToken cancellationToken = default)
    {
        return _getWithdrawals.SendAsync(count, before, after, messageId, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getBalance.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetParentOrdersCallAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        CancellationToken cancellationToken = default)
    {
        return _getParentOrders.SendAsync(productCode, count, before, after, parentOrderState, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetParentOrderCallAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        return _getParentOrder.SendAsync(parentOrderId, parentOrderAcceptanceId, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getCollateral.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getCollateralAccounts.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryCallAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return _getCollateralHistory.SendAsync(count, before, after, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetChildOrdersCallAsync(
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

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetExecutionsCallAsync(
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

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceHistoryCallAsync(
        string? currencyCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return _getBalanceHistory.SendAsync(currencyCode, count, before, after, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetPositionsCallAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _getPositions.SendAsync(productCode, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetTradingCommissionCallAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _getTradingCommission.SendAsync(productCode, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _sendChildOrder.SendAsync(bodyJson, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _sendParentOrder.SendAsync(bodyJson, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> CancelChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _cancelChildOrder.SendAsync(bodyJson, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _cancelAllChildOrders.SendAsync(bodyJson, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> CancelParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _cancelParentOrder.SendAsync(bodyJson, cancellationToken);
    }
}
