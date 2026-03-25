using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetCorporateLeverageNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var endpoint = new GetCorporateLeverageNativeEndpoint(new FakeGetCorporateLeverageProtocolEndpoint(() => SuccessProtocolCall("""{"current_max":2,"current_startdate":"2026-03-25T00:00:00+00:00","next_max":4,"next_startdate":"2026-03-26T00:00:00+00:00"}""")));

        var call = await endpoint.CallAsync(new GetCorporateLeverageRequest());

        Assert.True(call.IsSuccess);
        Assert.Equal(2m, call.Response!.CurrentMax);
        Assert.Equal(4m, call.Response.NextMax);
        Assert.Equal(new DateTimeOffset(2026, 3, 25, 0, 0, 0, TimeSpan.Zero), call.Response.CurrentStartDate);
        Assert.Equal(new DateTimeOffset(2026, 3, 26, 0, 0, 0, TimeSpan.Zero), call.Response.NextStartDate);
    }

    [Fact]
    public async Task CallAsync_AllowsMissingNextValues()
    {
        var endpoint = new GetCorporateLeverageNativeEndpoint(new FakeGetCorporateLeverageProtocolEndpoint(() => SuccessProtocolCall("""{"current_max":2,"current_startdate":"2026-03-25T00:00:00+00:00"}""")));

        var call = await endpoint.CallAsync(new GetCorporateLeverageRequest());

        Assert.True(call.IsSuccess);
        Assert.Null(call.Response!.NextMax);
        Assert.Null(call.Response.NextStartDate);
    }

    private static Call<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetCorporateLeverage", Method = "GET", Path = "/v1/getcorporateleverage", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetCorporateLeverage", Scope = "Public", Auth = "None" });
    }
}
