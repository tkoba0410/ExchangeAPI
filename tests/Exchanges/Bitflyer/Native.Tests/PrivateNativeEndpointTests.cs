using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class PrivateNativeEndpointTests
{
    [Fact]
    public async Task GetBalance_MapsTopLevelArray()
    {
        var body = """[{"currency_code":"JPY","amount":10,"available":5}]""";
        var endpoint = new GetBalanceNativeEndpoint(new FakeGetBalanceProtocolEndpoint(Success("GetBalance", "GET", "/v1/me/getbalance", body)));

        var call = await endpoint.CallAsync(new GetBalanceRequest());

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("JPY", call.Response![0].CurrencyCode);
    }

    [Fact]
    public async Task SendChildOrder_ReturnsSemantic_WhenLimitPriceMissing()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Limit,
            Side = OrderSides.Buy,
            Size = 0.01m,
            Price = null,
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendChildOrder_OmitsOptionalFields_ForMarketRequest()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => Success("SendChildOrder", "POST", "/v1/me/sendchildorder", """{"child_order_acceptance_id":"JRF123"}"""));
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Market,
            Side = OrderSides.Buy,
            Size = 0.01m,
            Price = null,
            MinuteToExpire = null,
            TimeInForce = null,
        });

        Assert.True(call.IsSuccess);
        Assert.NotNull(fake.LastBodyJson);
        Assert.DoesNotContain("price", fake.LastBodyJson!, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("minute_to_expire", fake.LastBodyJson!, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("time_in_force", fake.LastBodyJson!, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("JRF123", call.Response!.ChildOrderAcceptanceId);
    }

    [Fact]
    public async Task SendChildOrder_ReturnsSemantic_WhenChildOrderTypeIsInvalid()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "STOP",
            Side = OrderSides.Buy,
            Size = 0.01m,
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendChildOrder_ReturnsSemantic_WhenSideIsInvalid()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Market,
            Side = "HOLD",
            Size = 0.01m,
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendChildOrder_ReturnsSemantic_WhenMinuteToExpireExceedsMax()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Market,
            Side = OrderSides.Buy,
            Size = 0.01m,
            MinuteToExpire = 43201,
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendChildOrder_ReturnsSemantic_WhenTimeInForceIsInvalid()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Market,
            Side = OrderSides.Buy,
            Size = 0.01m,
            TimeInForce = "DAY",
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CancelChildOrder_ReturnsSemantic_WhenIdentifiersAreInvalid()
    {
        var fake = new FakeCancelChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new CancelChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderId = null,
            ChildOrderAcceptanceId = null,
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CancelChildOrder_AllowsEmptyBodyResponse()
    {
        var fake = new FakeCancelChildOrderProtocolEndpoint(_ => Success("CancelChildOrder", "POST", "/v1/me/cancelchildorder", string.Empty));
        var endpoint = new CancelChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderId = "JOR123",
        });

        Assert.True(call.IsSuccess);
    }

    private static Call<ProtocolRequest, ProtocolResponse> Success(string endpointId, string method, string path, string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = endpointId, Method = method, Path = path, Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PrivateEndpointModule, EndpointId = endpointId, Scope = "Private", Auth = "KeySecret" });
    }
}
