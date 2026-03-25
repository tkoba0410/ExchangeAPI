using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetFundingRateNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenProductCodeIsBlank()
    {
        var endpoint = new GetFundingRateNativeEndpoint(new FakeGetFundingRateProtocolEndpoint(_ => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetFundingRateRequest { ProductCode = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var endpoint = new GetFundingRateNativeEndpoint(new FakeGetFundingRateProtocolEndpoint(_ => SuccessProtocolCall("""{"current_funding_rate":0.001,"next_funding_rate_settledate":"2026-03-26T06:00:00"}""")));

        var call = await endpoint.CallAsync(new GetFundingRateRequest { ProductCode = "FX_BTC_JPY" });

        Assert.True(call.IsSuccess);
        Assert.Equal(0.001m, call.Response!.CurrentFundingRate);
        Assert.Equal(new DateTimeOffset(2026, 3, 26, 6, 0, 0, TimeSpan.Zero), call.Response.NextFundingRateSettleDate);
    }

    [Fact]
    public async Task CallAsync_NormalizesExplicitOffsetToUtc()
    {
        var endpoint = new GetFundingRateNativeEndpoint(new FakeGetFundingRateProtocolEndpoint(_ => SuccessProtocolCall("""{"current_funding_rate":0.001,"next_funding_rate_settledate":"2026-03-26T15:00:00+09:00"}""")));

        var call = await endpoint.CallAsync(new GetFundingRateRequest { ProductCode = "FX_BTC_JPY" });

        Assert.True(call.IsSuccess);
        Assert.Equal(new DateTimeOffset(2026, 3, 26, 6, 0, 0, TimeSpan.Zero), call.Response!.NextFundingRateSettleDate);
    }

    private static Call<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetFundingRate", Method = "GET", Path = "/v1/getfundingrate", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetFundingRate", Scope = "Public", Auth = "None" });
    }
}
