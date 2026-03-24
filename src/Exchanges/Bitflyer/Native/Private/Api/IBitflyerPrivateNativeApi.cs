using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;

public interface IBitflyerPrivateNativeApi
{
    Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        GetBalanceRequest request,
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

    Task<Call<CancelChildOrderRequest, Unit>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);
}
