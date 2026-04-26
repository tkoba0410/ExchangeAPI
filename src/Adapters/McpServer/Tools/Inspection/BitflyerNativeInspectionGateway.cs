using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Inspection;

public sealed class BitflyerNativeInspectionGateway : IBitflyerInspectionGateway
{
    private readonly IBitflyerPrivateNativeApi _privateApi;

    public BitflyerNativeInspectionGateway(IBitflyerPrivateNativeApi privateApi)
    {
        _privateApi = privateApi;
    }

    public Task<CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetCollateralAccountsAsync(new GetCollateralAccountsRequest(), cancellationToken);
    }

    public Task<CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetBalanceHistoryAsync(request, cancellationToken);
    }

    public Task<CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetCollateralHistoryAsync(request, cancellationToken);
    }

    public Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetChildOrdersAsync(request, cancellationToken);
    }
}
