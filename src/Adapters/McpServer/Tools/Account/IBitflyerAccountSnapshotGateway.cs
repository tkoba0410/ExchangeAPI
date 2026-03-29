using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Account;

public interface IBitflyerAccountSnapshotGateway
{
    Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersCallAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default);
}
