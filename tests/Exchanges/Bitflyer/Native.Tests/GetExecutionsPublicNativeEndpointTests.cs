using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetExecutionsPublicNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenProductCodeIsBlank()
    {
        var endpoint = new GetExecutionsPublicNativeEndpoint(new FakeGetExecutionsPublicProtocolEndpoint((productCode, count, before, after) => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetExecutionsPublicRequest { ProductCode = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_MapsTopLevelArray()
    {
        var body = """
        [
          {
            "id": 39287,
            "side": "BUY",
            "price": 31690,
            "size": 27.04,
            "exec_date": "2015-07-08T02:43:34.823",
            "buy_child_order_acceptance_id": "JRF20150707-200203-452209",
            "sell_child_order_acceptance_id": "JRF20150708-024334-060234"
          }
        ]
        """;

        var endpoint = new GetExecutionsPublicNativeEndpoint(new FakeGetExecutionsPublicProtocolEndpoint((productCode, count, before, after) => SuccessProtocolCall(body)));
        var call = await endpoint.CallAsync(new GetExecutionsPublicRequest { ProductCode = "BTC_JPY", Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal(39287L, call.Response![0].Id);
        Assert.Equal("BUY", call.Response[0].Side);
        Assert.Equal("JRF20150708-024334-060234", call.Response[0].SellChildOrderAcceptanceId);
    }

    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenCountIsInvalid()
    {
        var endpoint = new GetExecutionsPublicNativeEndpoint(new FakeGetExecutionsPublicProtocolEndpoint((productCode, count, before, after) => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetExecutionsPublicRequest { Count = 0 });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    private static Call<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetExecutionsPublic", Method = "GET", Path = "/v1/getexecutions", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetExecutionsPublic", Scope = "Public", Auth = "None" });
    }
}
