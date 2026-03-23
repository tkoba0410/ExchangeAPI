using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class NativeFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCall()
    {
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
        var api = new BitflyerPublicNativeApi(new FakeGetTickerNativeEndpoint(expected));

        var actual = await api.GetTickerCallAsync(new GetTickerRequest { ProductCode = null });

        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task PrivateFacade_ForwardsCalls()
    {
        var getBalance = CallFactory.Success(
            new GetBalanceRequest(),
            (IReadOnlyList<GetBalance.Item>)[new GetBalance.Item { CurrencyCode = "JPY", Amount = 1m, Available = 1m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBalance", Scope = "Private", Auth = "KeySecret" });
        var sendChildOrder = CallFactory.Success(
            new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = "MARKET", Side = "BUY", Size = 1m },
            new SendChildOrderResponse { ChildOrderAcceptanceId = "A" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "SendChildOrder", Scope = "Private", Auth = "KeySecret" });
        var cancelChildOrder = CallFactory.Success(
            new CancelChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderId = "X" },
            new Unit(),
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "CancelChildOrder", Scope = "Private", Auth = "KeySecret" });

        var api = new BitflyerPrivateNativeApi(
            new FakeGetBalanceNativeEndpoint(getBalance),
            new FakeSendChildOrderNativeEndpoint(sendChildOrder),
            new FakeCancelChildOrderNativeEndpoint(cancelChildOrder));

        Assert.Same(getBalance, await api.GetBalanceCallAsync(new GetBalanceRequest()));
        Assert.Same(sendChildOrder, await api.SendChildOrderCallAsync(new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = "MARKET", Side = "BUY", Size = 1m }));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderCallAsync(new CancelChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderId = "X" }));
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
}
