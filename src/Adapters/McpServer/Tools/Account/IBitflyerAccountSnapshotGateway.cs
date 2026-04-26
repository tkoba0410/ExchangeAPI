using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Account;

public interface IBitflyerAccountSnapshotGateway
{
    Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(
        CancellationToken cancellationToken = default);

    Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(
        CancellationToken cancellationToken = default);

    Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(
        CancellationToken cancellationToken = default);
}
