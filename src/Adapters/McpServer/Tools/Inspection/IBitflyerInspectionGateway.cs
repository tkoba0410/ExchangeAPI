using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Inspection;

public interface IBitflyerInspectionGateway
{
    Task<CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default);

    Task<CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);
}
