using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
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
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class ProtocolFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCalls()
    {
        var getBoard = TestCall();
        var getTicker = TestCall();
        var api = new BitflyerPublicProtocolApi(
            new FakeGetBoardProtocolEndpoint(getBoard),
            new FakeGetTickerProtocolEndpoint(getTicker));

        var actualBoard = await api.GetBoardCallAsync("BTC_JPY");
        var actualTicker = await api.GetTickerCallAsync("BTC_JPY");

        Assert.Same(getBoard, actualBoard);
        Assert.Same(getTicker, actualTicker);
    }

    [Fact]
    public async Task PrivateFacade_ForwardsCalls()
    {
        var getBalance = TestCall();
        var getCollateral = TestCall();
        var getCollateralAccounts = TestCall();
        var getCollateralHistory = TestCall();
        var getChildOrders = TestCall();
        var getExecutions = TestCall();
        var getPositions = TestCall();
        var sendChildOrder = TestCall();
        var cancelChildOrder = TestCall();
        var cancelAllChildOrders = TestCall();
        var api = new BitflyerPrivateProtocolApi(
            new FakeGetBalanceProtocolEndpoint(getBalance),
            new FakeGetCollateralProtocolEndpoint(getCollateral),
            new FakeGetCollateralAccountsProtocolEndpoint(getCollateralAccounts),
            new FakeGetCollateralHistoryProtocolEndpoint(getCollateralHistory),
            new FakeGetChildOrdersProtocolEndpoint(getChildOrders),
            new FakeGetExecutionsProtocolEndpoint(getExecutions),
            new FakeGetPositionsProtocolEndpoint(getPositions),
            new FakeSendChildOrderProtocolEndpoint(sendChildOrder),
            new FakeCancelChildOrderProtocolEndpoint(cancelChildOrder),
            new FakeCancelAllChildOrdersProtocolEndpoint(cancelAllChildOrders));

        Assert.Same(getBalance, await api.GetBalanceCallAsync());
        Assert.Same(getCollateral, await api.GetCollateralCallAsync());
        Assert.Same(getCollateralAccounts, await api.GetCollateralAccountsCallAsync());
        Assert.Same(getCollateralHistory, await api.GetCollateralHistoryCallAsync(10, null, null));
        Assert.Same(getChildOrders, await api.GetChildOrdersCallAsync("BTC_JPY", 10, null, null, "COMPLETED", null, null, null));
        Assert.Same(getExecutions, await api.GetExecutionsCallAsync("BTC_JPY", 10, null, null, null, null));
        Assert.Same(getPositions, await api.GetPositionsCallAsync("FX_BTC_JPY"));
        Assert.Same(sendChildOrder, await api.SendChildOrderCallAsync("{}"));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderCallAsync("{}"));
        Assert.Same(cancelAllChildOrders, await api.CancelAllChildOrdersCallAsync("{}"));
    }

    private static Call<ProtocolRequest, ProtocolResponse> TestCall()
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "E", Method = "GET", Path = "/", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = "{}" },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "E", Scope = "Public", Auth = "None" });
    }

    private sealed class FakeGetBoardProtocolEndpoint : IGetBoardProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBoardProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTickerProtocolEndpoint : IGetTickerProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetTickerProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceProtocolEndpoint : IGetBalanceProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBalanceProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralProtocolEndpoint : IGetCollateralProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralAccountsProtocolEndpoint : IGetCollateralAccountsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralAccountsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralHistoryProtocolEndpoint : IGetCollateralHistoryProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralHistoryProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChildOrdersProtocolEndpoint : IGetChildOrdersProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetChildOrdersProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;

        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
            string? productCode,
            int? count,
            long? before,
            long? after,
            string? childOrderState,
            string? childOrderId,
            string? childOrderAcceptanceId,
            string? parentOrderId,
            CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsProtocolEndpoint : IGetExecutionsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetExecutionsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;

        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
            string productCode,
            int? count,
            long? before,
            long? after,
            string? childOrderId,
            string? childOrderAcceptanceId,
            CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPositionsProtocolEndpoint : IGetPositionsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetPositionsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendChildOrderProtocolEndpoint : ISendChildOrderProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeSendChildOrderProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelChildOrderProtocolEndpoint : ICancelChildOrderProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelChildOrderProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelAllChildOrdersProtocolEndpoint : ICancelAllChildOrdersProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelAllChildOrdersProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }
}
