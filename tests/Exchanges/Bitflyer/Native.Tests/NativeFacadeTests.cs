using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
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
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class NativeFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCalls()
    {
        var getMarkets = CallFactory.Success(
            new GetMarketsRequest(),
            (IReadOnlyList<GetMarkets.Item>)[new GetMarkets.Item { ProductCode = "BTC_JPY", MarketType = "Spot" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetMarkets", Scope = "Public", Auth = "None" });
        var getBoard = CallFactory.Success(
            new GetBoardRequest { ProductCode = null },
            new GetBoardResponse
            {
                MidPrice = 1m,
                Bids = [new GetBoardLevel { Price = 1m, Size = 1m }],
                Asks = [new GetBoardLevel { Price = 2m, Size = 1m }],
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetBoard", Scope = "Public", Auth = "None" });
        var getExecutions = CallFactory.Success(
            new GetExecutionsPublicRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetExecutionsPublic.Item>)[new GetExecutionsPublic.Item
            {
                Id = 1,
                Side = "BUY",
                Price = 1m,
                Size = 1m,
                ExecDate = DateTimeOffset.UnixEpoch,
                BuyChildOrderAcceptanceId = "JRF-BUY",
                SellChildOrderAcceptanceId = "JRF-SELL",
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetExecutionsPublic", Scope = "Public", Auth = "None" });
        var expected = CallFactory.Success(
            new GetTickerRequest { ProductCode = null },
            new GetTickerResponse
            {
                ProductCode = "BTC_JPY",
                State = "RUNNING",
                Timestamp = DateTimeOffset.UtcNow,
                TickId = 1,
                BestBid = 1,
                BestAsk = 1,
                BestBidSize = 1,
                BestAskSize = 1,
                TotalBidDepth = 1,
                TotalAskDepth = 1,
                MarketBidSize = 0,
                MarketAskSize = 0,
                Ltp = 1,
                Volume = 1,
                VolumeByProduct = 1,
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetTicker", Scope = "Public", Auth = "None" });
        var api = new BitflyerPublicNativeApi(
            new FakeGetMarketsNativeEndpoint(getMarkets),
            new FakeGetBoardNativeEndpoint(getBoard),
            new FakeGetExecutionsPublicNativeEndpoint(getExecutions),
            new FakeGetTickerNativeEndpoint(expected));

        var actualMarkets = await api.GetMarketsCallAsync(new GetMarketsRequest());
        var actualBoard = await api.GetBoardCallAsync(new GetBoardRequest { ProductCode = null });
        var actualExecutions = await api.GetExecutionsCallAsync(new GetExecutionsPublicRequest { ProductCode = "BTC_JPY", Count = 10 });
        var actualTicker = await api.GetTickerCallAsync(new GetTickerRequest { ProductCode = null });

        Assert.Same(getMarkets, actualMarkets);
        Assert.Same(getBoard, actualBoard);
        Assert.Same(getExecutions, actualExecutions);
        Assert.Same(expected, actualTicker);
    }

    [Fact]
    public async Task PrivateFacade_ForwardsCalls()
    {
        var getBalance = CallFactory.Success(
            new GetBalanceRequest(),
            (IReadOnlyList<GetBalance.Item>)[new GetBalance.Item { CurrencyCode = "JPY", Amount = 1m, Available = 1m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBalance", Scope = "Private", Auth = "KeySecret" });
        var getCollateral = CallFactory.Success(
            new GetCollateralRequest(),
            new GetCollateralResponse
            {
                Collateral = 10m,
                OpenPositionPnl = -1m,
                RequireCollateral = 2m,
                KeepRate = 5m,
                MarginCallAmount = null,
                MarginCallDueDate = null,
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCollateral", Scope = "Private", Auth = "KeySecret" });
        var getCollateralAccounts = CallFactory.Success(
            new GetCollateralAccountsRequest(),
            (IReadOnlyList<GetCollateralAccounts.Item>)[new GetCollateralAccounts.Item { CurrencyCode = "JPY", Amount = 10m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCollateralAccounts", Scope = "Private", Auth = "KeySecret" });
        var getCollateralHistory = CallFactory.Success(
            new GetCollateralHistoryRequest { Count = 10 },
            (IReadOnlyList<GetCollateralHistory.Item>)[new GetCollateralHistory.Item
            {
                Id = 1,
                CurrencyCode = "JPY",
                Change = -1m,
                Amount = 10m,
                ReasonCode = "CLEARING_COLL",
                Date = DateTimeOffset.UnixEpoch,
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCollateralHistory", Scope = "Private", Auth = "KeySecret" });
        var getChildOrders = CallFactory.Success(
            new GetChildOrdersRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetChildOrders.Item>)[new GetChildOrders.Item
            {
                Id = 1,
                ChildOrderId = "JOR1",
                ProductCode = "BTC_JPY",
                Side = "BUY",
                ChildOrderType = "LIMIT",
                Price = 1m,
                AveragePrice = 1m,
                Size = 1m,
                ChildOrderState = "COMPLETED",
                ExpireDate = DateTimeOffset.UnixEpoch,
                ChildOrderDate = DateTimeOffset.UnixEpoch,
                ChildOrderAcceptanceId = "JRF1",
                OutstandingSize = 0m,
                CancelSize = 0m,
                ExecutedSize = 1m,
                TotalCommission = 0m,
                TimeInForce = "GTC",
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetChildOrders", Scope = "Private", Auth = "KeySecret" });
        var getExecutions = CallFactory.Success(
            new GetExecutionsRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetExecutions.Item>)[new GetExecutions.Item
            {
                Id = 1,
                ChildOrderId = "JOR1",
                Side = "BUY",
                Price = 1m,
                Size = 1m,
                Commission = 0m,
                ExecDate = DateTimeOffset.UnixEpoch,
                ChildOrderAcceptanceId = "JRF1",
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetExecutionsPrivate", Scope = "Private", Auth = "KeySecret" });
        var getPositions = CallFactory.Success(
            new GetPositionsRequest { ProductCode = "FX_BTC_JPY" },
            (IReadOnlyList<GetPositions.Item>)[new GetPositions.Item
            {
                ProductCode = "FX_BTC_JPY",
                Side = "BUY",
                Price = 1m,
                Size = 1m,
                Commission = 0m,
                SwapPointAccumulate = 0m,
                RequireCollateral = 1m,
                OpenDate = DateTimeOffset.UnixEpoch,
                Leverage = 2m,
                Pnl = 0m,
                Sfd = 0m,
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetPositions", Scope = "Private", Auth = "KeySecret" });
        var getTradingCommission = CallFactory.Success(
            new GetTradingCommissionRequest { ProductCode = "BTC_JPY" },
            new GetTradingCommissionResponse { CommissionRate = 0.001m },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetTradingCommission", Scope = "Private", Auth = "KeySecret" });
        var sendChildOrder = CallFactory.Success(
            new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = "MARKET", Side = "BUY", Size = 1m },
            new SendChildOrderResponse { ChildOrderAcceptanceId = "A" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "SendChildOrder", Scope = "Private", Auth = "KeySecret" });
        var cancelChildOrder = CallFactory.Success(
            new CancelChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderId = "X" },
            new Unit(),
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "CancelChildOrder", Scope = "Private", Auth = "KeySecret" });
        var cancelAllChildOrders = CallFactory.Success(
            new CancelAllChildOrdersRequest { ProductCode = "BTC_JPY" },
            new Unit(),
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "CancelAllChildOrders", Scope = "Private", Auth = "KeySecret" });

        var api = new BitflyerPrivateNativeApi(
            new FakeGetBalanceNativeEndpoint(getBalance),
            new FakeGetCollateralNativeEndpoint(getCollateral),
            new FakeGetCollateralAccountsNativeEndpoint(getCollateralAccounts),
            new FakeGetCollateralHistoryNativeEndpoint(getCollateralHistory),
            new FakeGetChildOrdersNativeEndpoint(getChildOrders),
            new FakeGetExecutionsNativeEndpoint(getExecutions),
            new FakeGetPositionsNativeEndpoint(getPositions),
            new FakeGetTradingCommissionNativeEndpoint(getTradingCommission),
            new FakeSendChildOrderNativeEndpoint(sendChildOrder),
            new FakeCancelChildOrderNativeEndpoint(cancelChildOrder),
            new FakeCancelAllChildOrdersNativeEndpoint(cancelAllChildOrders));

        Assert.Same(getBalance, await api.GetBalanceCallAsync(new GetBalanceRequest()));
        Assert.Same(getCollateral, await api.GetCollateralCallAsync(new GetCollateralRequest()));
        Assert.Same(getCollateralAccounts, await api.GetCollateralAccountsCallAsync(new GetCollateralAccountsRequest()));
        Assert.Same(getCollateralHistory, await api.GetCollateralHistoryCallAsync(new GetCollateralHistoryRequest { Count = 10 }));
        Assert.Same(getChildOrders, await api.GetChildOrdersCallAsync(new GetChildOrdersRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getExecutions, await api.GetExecutionsCallAsync(new GetExecutionsRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getPositions, await api.GetPositionsCallAsync(new GetPositionsRequest { ProductCode = "FX_BTC_JPY" }));
        Assert.Same(getTradingCommission, await api.GetTradingCommissionCallAsync(new GetTradingCommissionRequest { ProductCode = "BTC_JPY" }));
        Assert.Same(sendChildOrder, await api.SendChildOrderCallAsync(new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = "MARKET", Side = "BUY", Size = 1m }));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderCallAsync(new CancelChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderId = "X" }));
        Assert.Same(cancelAllChildOrders, await api.CancelAllChildOrdersCallAsync(new CancelAllChildOrdersRequest { ProductCode = "BTC_JPY" }));
    }

    private sealed class FakeGetMarketsNativeEndpoint : IGetMarketsNativeEndpoint
    {
        private readonly Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>> _call;
        public FakeGetMarketsNativeEndpoint(Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>> call) => _call = call;
        public Task<Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> CallAsync(GetMarketsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardNativeEndpoint : IGetBoardNativeEndpoint
    {
        private readonly Call<GetBoardRequest, GetBoardResponse> _call;
        public FakeGetBoardNativeEndpoint(Call<GetBoardRequest, GetBoardResponse> call) => _call = call;
        public Task<Call<GetBoardRequest, GetBoardResponse>> CallAsync(GetBoardRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsPublicNativeEndpoint : IGetExecutionsPublicNativeEndpoint
    {
        private readonly Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>> _call;
        public FakeGetExecutionsPublicNativeEndpoint(Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>> call) => _call = call;
        public Task<Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> CallAsync(GetExecutionsPublicRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTickerNativeEndpoint : IGetTickerNativeEndpoint
    {
        private readonly Call<GetTickerRequest, GetTickerResponse> _call;
        public FakeGetTickerNativeEndpoint(Call<GetTickerRequest, GetTickerResponse> call) => _call = call;
        public Task<Call<GetTickerRequest, GetTickerResponse>> CallAsync(GetTickerRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceNativeEndpoint : IGetBalanceNativeEndpoint
    {
        private readonly Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> _call;
        public FakeGetBalanceNativeEndpoint(Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> call) => _call = call;
        public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> CallAsync(GetBalanceRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralNativeEndpoint : IGetCollateralNativeEndpoint
    {
        private readonly Call<GetCollateralRequest, GetCollateralResponse> _call;
        public FakeGetCollateralNativeEndpoint(Call<GetCollateralRequest, GetCollateralResponse> call) => _call = call;
        public Task<Call<GetCollateralRequest, GetCollateralResponse>> CallAsync(GetCollateralRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralAccountsNativeEndpoint : IGetCollateralAccountsNativeEndpoint
    {
        private readonly Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>> _call;
        public FakeGetCollateralAccountsNativeEndpoint(Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>> call) => _call = call;
        public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> CallAsync(GetCollateralAccountsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralHistoryNativeEndpoint : IGetCollateralHistoryNativeEndpoint
    {
        private readonly Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>> _call;
        public FakeGetCollateralHistoryNativeEndpoint(Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>> call) => _call = call;
        public Task<Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> CallAsync(GetCollateralHistoryRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChildOrdersNativeEndpoint : IGetChildOrdersNativeEndpoint
    {
        private readonly Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> _call;
        public FakeGetChildOrdersNativeEndpoint(Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> call) => _call = call;
        public Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> CallAsync(GetChildOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsNativeEndpoint : IGetExecutionsNativeEndpoint
    {
        private readonly Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>> _call;
        public FakeGetExecutionsNativeEndpoint(Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>> call) => _call = call;
        public Task<Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsync(GetExecutionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPositionsNativeEndpoint : IGetPositionsNativeEndpoint
    {
        private readonly Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> _call;
        public FakeGetPositionsNativeEndpoint(Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> call) => _call = call;
        public Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> CallAsync(GetPositionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTradingCommissionNativeEndpoint : IGetTradingCommissionNativeEndpoint
    {
        private readonly Call<GetTradingCommissionRequest, GetTradingCommissionResponse> _call;
        public FakeGetTradingCommissionNativeEndpoint(Call<GetTradingCommissionRequest, GetTradingCommissionResponse> call) => _call = call;
        public Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> CallAsync(GetTradingCommissionRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendChildOrderNativeEndpoint : ISendChildOrderNativeEndpoint
    {
        private readonly Call<SendChildOrderRequest, SendChildOrderResponse> _call;
        public FakeSendChildOrderNativeEndpoint(Call<SendChildOrderRequest, SendChildOrderResponse> call) => _call = call;
        public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(SendChildOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelChildOrderNativeEndpoint : ICancelChildOrderNativeEndpoint
    {
        private readonly Call<CancelChildOrderRequest, Unit> _call;
        public FakeCancelChildOrderNativeEndpoint(Call<CancelChildOrderRequest, Unit> call) => _call = call;
        public Task<Call<CancelChildOrderRequest, Unit>> CallAsync(CancelChildOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelAllChildOrdersNativeEndpoint : ICancelAllChildOrdersNativeEndpoint
    {
        private readonly Call<CancelAllChildOrdersRequest, Unit> _call;
        public FakeCancelAllChildOrdersNativeEndpoint(Call<CancelAllChildOrdersRequest, Unit> call) => _call = call;
        public Task<Call<CancelAllChildOrdersRequest, Unit>> CallAsync(CancelAllChildOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }
}
