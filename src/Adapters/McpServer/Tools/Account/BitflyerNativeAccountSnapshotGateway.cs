using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Account;

public sealed class BitflyerNativeAccountSnapshotGateway : IBitflyerAccountSnapshotGateway
{
    private readonly IBitflyerPrivateNativeApi _privateApi;

    public BitflyerNativeAccountSnapshotGateway(IBitflyerPrivateNativeApi privateApi)
    {
        _privateApi = privateApi;
    }

    public Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetBalanceAsync(new GetBalanceRequest(), cancellationToken);
    }

    public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetCollateralAsync(new GetCollateralRequest(), cancellationToken);
    }

    public Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetChildOrdersAsync(
            new GetChildOrdersRequest
            {
                ProductCode = productCode,
                ChildOrderState = ChildOrderStates.Active,
            },
            cancellationToken);
    }

    public Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetPositionsAsync(
            new GetPositionsRequest
            {
                ProductCode = productCode,
            },
            cancellationToken);
    }

    public Task<CallResult<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetPermissionsAsync(new GetPermissionsRequest(), cancellationToken);
    }
}
