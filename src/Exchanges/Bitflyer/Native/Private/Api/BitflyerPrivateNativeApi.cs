using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;

public sealed class BitflyerPrivateNativeApi : IBitflyerPrivateNativeApi
{
    private readonly IGetBalanceNativeEndpoint _getBalance;
    private readonly IGetCollateralNativeEndpoint _getCollateral;
    private readonly IGetCollateralAccountsNativeEndpoint _getCollateralAccounts;
    private readonly IGetCollateralHistoryNativeEndpoint _getCollateralHistory;
    private readonly IGetChildOrdersNativeEndpoint _getChildOrders;
    private readonly IGetExecutionsNativeEndpoint _getExecutions;
    private readonly IGetPositionsNativeEndpoint _getPositions;
    private readonly ISendChildOrderNativeEndpoint _sendChildOrder;
    private readonly ICancelChildOrderNativeEndpoint _cancelChildOrder;
    private readonly ICancelAllChildOrdersNativeEndpoint _cancelAllChildOrders;

    public BitflyerPrivateNativeApi(
        IGetBalanceNativeEndpoint getBalance,
        IGetCollateralNativeEndpoint getCollateral,
        IGetCollateralAccountsNativeEndpoint getCollateralAccounts,
        IGetCollateralHistoryNativeEndpoint getCollateralHistory,
        IGetChildOrdersNativeEndpoint getChildOrders,
        IGetExecutionsNativeEndpoint getExecutions,
        IGetPositionsNativeEndpoint getPositions,
        ISendChildOrderNativeEndpoint sendChildOrder,
        ICancelChildOrderNativeEndpoint cancelChildOrder,
        ICancelAllChildOrdersNativeEndpoint cancelAllChildOrders)
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

    public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBalance.CallAsync(request, cancellationToken);
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

    public Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getPositions.CallAsync(request, cancellationToken);
    }

    public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _sendChildOrder.CallAsync(request, cancellationToken);
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
}
