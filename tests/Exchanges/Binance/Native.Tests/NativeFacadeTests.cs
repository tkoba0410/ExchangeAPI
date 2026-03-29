using ExchangeApi.Exchanges.Binance.Native.Public.Api;
using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Exchanges.Binance.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Tests.Exchanges.Binance.Native.Tests;

public sealed class NativeFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCall()
    {
        var expected = CallFactory.Success(
            new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h },
            (IReadOnlyList<GetKlines.Item>)[],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetKlines", Scope = "Public", Auth = "None" });

        var api = new BinancePublicNativeApi(new FakeGetKlinesNativeEndpoint(expected));

        var actual = await api.GetKlinesCallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h });

        Assert.Same(expected, actual);
    }

    private sealed class FakeGetKlinesNativeEndpoint : IGetKlinesNativeEndpoint
    {
        private readonly Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>> _call;

        public FakeGetKlinesNativeEndpoint(Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>> call)
        {
            _call = call;
        }

        public Task<Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> CallAsync(
            GetKlinesRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_call);
        }
    }
}
