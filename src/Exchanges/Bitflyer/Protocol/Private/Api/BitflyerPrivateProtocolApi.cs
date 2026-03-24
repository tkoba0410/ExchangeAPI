using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;

public sealed class BitflyerPrivateProtocolApi : IBitflyerPrivateProtocolApi
{
    private readonly IGetBalanceProtocolEndpoint _getBalance;
    private readonly IGetCollateralProtocolEndpoint _getCollateral;
    private readonly IGetCollateralAccountsProtocolEndpoint _getCollateralAccounts;
    private readonly IGetCollateralHistoryProtocolEndpoint _getCollateralHistory;
    private readonly IGetChildOrdersProtocolEndpoint _getChildOrders;
    private readonly IGetExecutionsProtocolEndpoint _getExecutions;
    private readonly IGetPositionsProtocolEndpoint _getPositions;
    private readonly ISendChildOrderProtocolEndpoint _sendChildOrder;
    private readonly ICancelChildOrderProtocolEndpoint _cancelChildOrder;
    private readonly ICancelAllChildOrdersProtocolEndpoint _cancelAllChildOrders;

    public BitflyerPrivateProtocolApi(
        IGetBalanceProtocolEndpoint getBalance,
        IGetCollateralProtocolEndpoint getCollateral,
        IGetCollateralAccountsProtocolEndpoint getCollateralAccounts,
        IGetCollateralHistoryProtocolEndpoint getCollateralHistory,
        IGetChildOrdersProtocolEndpoint getChildOrders,
        IGetExecutionsProtocolEndpoint getExecutions,
        IGetPositionsProtocolEndpoint getPositions,
        ISendChildOrderProtocolEndpoint sendChildOrder,
        ICancelChildOrderProtocolEndpoint cancelChildOrder,
        ICancelAllChildOrdersProtocolEndpoint cancelAllChildOrders)
    {
        _getBalance = getBalance;
        _getCollateral = getCollateral;
        _getCollateralAccounts = getCollateralAccounts;
        _getCollateralHistory = getCollateralHistory;
        _getChildOrders = getChildOrders;
        _getExecutions = getExecutions;
        _getPositions = getPositions;
        _sendChildOrder = sendChildOrder;
        _cancelChildOrder = cancelChildOrder;
        _cancelAllChildOrders = cancelAllChildOrders;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getBalance.SendAsync(cancellationToken);
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

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetPositionsCallAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _getPositions.SendAsync(productCode, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _sendChildOrder.SendAsync(bodyJson, cancellationToken);
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
}
