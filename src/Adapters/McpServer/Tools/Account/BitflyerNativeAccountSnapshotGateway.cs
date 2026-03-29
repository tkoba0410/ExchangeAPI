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

    public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetBalanceCallAsync(new GetBalanceRequest(), cancellationToken);
    }

    public Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetCollateralCallAsync(new GetCollateralRequest(), cancellationToken);
    }

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersCallAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetChildOrdersCallAsync(
            new GetChildOrdersRequest
            {
                ProductCode = productCode,
                ChildOrderState = ChildOrderStates.Active,
            },
            cancellationToken);
    }

    public Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetPositionsCallAsync(
            new GetPositionsRequest
            {
                ProductCode = productCode,
            },
            cancellationToken);
    }

    public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetPermissionsCallAsync(new GetPermissionsRequest(), cancellationToken);
    }
}
