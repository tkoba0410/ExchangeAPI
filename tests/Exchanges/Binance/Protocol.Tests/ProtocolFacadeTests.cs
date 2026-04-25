using ExchangeApi.Exchanges.Binance.Protocol.Public.Api;
using ExchangeApi.Exchanges.Binance.Protocol.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Binance.Protocol.Tests;

public sealed class ProtocolFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCall()
    {
        var expected = TestCall();
        var api = new BinancePublicProtocolApi(new FakeGetKlinesProtocolEndpoint(expected));

        var actual = await api.GetKlinesAsync("BTCJPY", "1h");

        Assert.Same(expected, actual);
    }

    private static CallResult<ProtocolRequest, ProtocolResponse> TestCall()
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "E", Method = "GET", Path = "/", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = "[]" },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "E", Scope = "Public", Auth = "None" });
    }

    private sealed class FakeGetKlinesProtocolEndpoint : IGetKlinesProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;

        public FakeGetKlinesProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call)
        {
            _call = call;
        }

        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
            string symbol,
            string interval,
            long? startTime = null,
            long? endTime = null,
            string? timeZone = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_call);
        }
    }
}
