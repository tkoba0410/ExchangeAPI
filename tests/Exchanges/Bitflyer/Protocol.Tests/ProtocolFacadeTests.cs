using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class ProtocolFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCall()
    {
        var expected = TestCall();
        var api = new BitflyerPublicProtocolApi(new FakeGetTickerProtocolEndpoint(expected));

        var actual = await api.GetTickerCallAsync("BTC_JPY");

        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task PrivateFacade_ForwardsCalls()
    {
        var getBalance = TestCall();
        var sendChildOrder = TestCall();
        var cancelChildOrder = TestCall();
        var api = new BitflyerPrivateProtocolApi(
            new FakeGetBalanceProtocolEndpoint(getBalance),
            new FakeSendChildOrderProtocolEndpoint(sendChildOrder),
            new FakeCancelChildOrderProtocolEndpoint(cancelChildOrder));

        Assert.Same(getBalance, await api.GetBalanceCallAsync());
        Assert.Same(sendChildOrder, await api.SendChildOrderCallAsync("{}"));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderCallAsync("{}"));
    }

    private static Call<ProtocolRequest, ProtocolResponse> TestCall()
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "E", Method = "GET", Path = "/", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = "{}" },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "E", Scope = "Public", Auth = "None" });
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
}
