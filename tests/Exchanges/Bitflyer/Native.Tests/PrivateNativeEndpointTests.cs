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
    public async Task GetCollateral_MapsObject()
    {
        var body = """
            {"collateral":100000,"open_position_pnl":-715,"require_collateral":19857,"keep_rate":5.000,"margin_call_amount":1000000,"margin_call_due_date":"2021-09-01T08:00:00"}
            """;
        var endpoint = new GetCollateralNativeEndpoint(new FakeGetCollateralProtocolEndpoint(Success("GetCollateral", "GET", "/v1/me/getcollateral", body)));

        var call = await endpoint.CallAsync(new GetCollateralRequest());

        Assert.True(call.IsSuccess);
        Assert.NotNull(call.Response);
        Assert.Equal(100000m, call.Response!.Collateral);
        Assert.Equal(-715m, call.Response.OpenPositionPnl);
        Assert.Equal(1000000m, call.Response.MarginCallAmount);
        Assert.NotNull(call.Response.MarginCallDueDate);
        Assert.Equal(new DateTimeOffset(2021, 9, 1, 8, 0, 0, TimeSpan.Zero), call.Response.MarginCallDueDate);
    }

    [Fact]
    public async Task GetCollateralAccounts_MapsTopLevelArray()
    {
        var body = """[{"currency_code":"JPY","amount":10000},{"currency_code":"BTC","amount":1.23}]""";
        var endpoint = new GetCollateralAccountsNativeEndpoint(new FakeGetCollateralAccountsProtocolEndpoint(Success("GetCollateralAccounts", "GET", "/v1/me/getcollateralaccounts", body)));

        var call = await endpoint.CallAsync(new GetCollateralAccountsRequest());

        Assert.True(call.IsSuccess);
        Assert.Equal(2, call.Response!.Count);
        Assert.Equal("JPY", call.Response![0].CurrencyCode);
        Assert.Equal(1.23m, call.Response[1].Amount);
    }

    [Fact]
    public async Task GetPositions_MapsTopLevelArray()
    {
        var body = """
            [{"product_code":"FX_BTC_JPY","side":"BUY","price":36000,"size":10,"commission":0,"swap_point_accumulate":-35,"require_collateral":120000,"open_date":"2015-11-03T10:04:45.011","leverage":3,"pnl":965,"sfd":-0.5}]
            """;
        var endpoint = new GetPositionsNativeEndpoint(new FakeGetPositionsProtocolEndpoint(_ => Success("GetPositions", "GET", "/v1/me/getpositions", body)));

        var call = await endpoint.CallAsync(new GetPositionsRequest { ProductCode = ProductCodes.FxBtcJpy });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal(ProductCodes.FxBtcJpy, call.Response![0].ProductCode);
        Assert.Equal("BUY", call.Response[0].Side);
        Assert.Equal(36000m, call.Response[0].Price);
        Assert.Equal(-0.5m, call.Response[0].Sfd);
        Assert.Equal(new DateTimeOffset(2015, 11, 3, 10, 4, 45, 11, TimeSpan.Zero), call.Response[0].OpenDate);
    }

    [Fact]
    public async Task GetPositions_ReturnsSemantic_WhenProductCodeIsInvalid()
    {
        var endpoint = new GetPositionsNativeEndpoint(new FakeGetPositionsProtocolEndpoint(_ => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetPositionsRequest { ProductCode = ProductCodes.BtcJpy });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetChildOrders_MapsTopLevelArray()
    {
        var body = """
            [{"id":138398,"child_order_id":"JOR20150707-084555-022523","product_code":"BTC_JPY","side":"BUY","child_order_type":"LIMIT","price":30000,"average_price":30000,"size":0.1,"child_order_state":"COMPLETED","expire_date":"2015-07-14T07:25:52","child_order_date":"2015-07-07T08:45:53","child_order_acceptance_id":"JRF20150707-084552-031927","outstanding_size":0,"cancel_size":0,"executed_size":0.1,"total_commission":0,"time_in_force":"GTC"}]
            """;
        var endpoint = new GetChildOrdersNativeEndpoint(new FakeGetChildOrdersProtocolEndpoint((productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId) => Success("GetChildOrders", "GET", "/v1/me/getchildorders", body)));

        var call = await endpoint.CallAsync(new GetChildOrdersRequest { ProductCode = ProductCodes.BtcJpy, Count = 10, ChildOrderState = ChildOrderStates.Completed });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal(138398L, call.Response![0].Id);
        Assert.Equal("JOR20150707-084555-022523", call.Response[0].ChildOrderId);
        Assert.Equal("COMPLETED", call.Response[0].ChildOrderState);
        Assert.Equal(0.1m, call.Response[0].ExecutedSize);
        Assert.Equal(new DateTimeOffset(2015, 7, 14, 7, 25, 52, TimeSpan.Zero), call.Response[0].ExpireDate);
        Assert.Equal(new DateTimeOffset(2015, 7, 7, 8, 45, 53, TimeSpan.Zero), call.Response[0].ChildOrderDate);
    }

    [Fact]
    public async Task GetChildOrders_ReturnsSemantic_WhenCountIsInvalid()
    {
        var endpoint = new GetChildOrdersNativeEndpoint(new FakeGetChildOrdersProtocolEndpoint((productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetChildOrdersRequest { Count = 0 });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetChildOrders_ReturnsSemantic_WhenChildOrderStateIsInvalid()
    {
        var endpoint = new GetChildOrdersNativeEndpoint(new FakeGetChildOrdersProtocolEndpoint((productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetChildOrdersRequest { ChildOrderState = "PENDING" });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetExecutions_MapsTopLevelArray()
    {
        var body = """
            [{"id":37233,"child_order_id":"JOR20150707-060559-021935","side":"BUY","price":33470,"size":0.01,"commission":0,"exec_date":"2015-07-07T09:57:40.397","child_order_acceptance_id":"JRF20150707-060559-396699"}]
            """;
        var endpoint = new GetExecutionsNativeEndpoint(new FakeGetExecutionsProtocolEndpoint((productCode, count, before, after, childOrderId, childOrderAcceptanceId) => Success("GetExecutionsPrivate", "GET", "/v1/me/getexecutions", body)));

        var call = await endpoint.CallAsync(new GetExecutionsRequest { ProductCode = ProductCodes.BtcJpy, Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal(37233L, call.Response![0].Id);
        Assert.Equal("JOR20150707-060559-021935", call.Response[0].ChildOrderId);
        Assert.Equal("BUY", call.Response[0].Side);
        Assert.Equal(33470m, call.Response[0].Price);
        Assert.Equal(new DateTimeOffset(2015, 7, 7, 9, 57, 40, 397, TimeSpan.Zero), call.Response[0].ExecDate);
    }

    [Fact]
    public async Task GetExecutions_ReturnsSemantic_WhenProductCodeIsMissing()
    {
        var endpoint = new GetExecutionsNativeEndpoint(new FakeGetExecutionsProtocolEndpoint((productCode, count, before, after, childOrderId, childOrderAcceptanceId) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetExecutionsRequest { ProductCode = "" });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetExecutions_ReturnsSemantic_WhenCountIsInvalid()
    {
        var endpoint = new GetExecutionsNativeEndpoint(new FakeGetExecutionsProtocolEndpoint((productCode, count, before, after, childOrderId, childOrderAcceptanceId) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetExecutionsRequest { ProductCode = ProductCodes.BtcJpy, Count = 0 });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetCollateralHistory_MapsTopLevelArray()
    {
        var body = """
            [{"id":4995,"currency_code":"JPY","change":-6,"amount":-6,"reason_code":"CLEARING_COLL","date":"2017-05-18T02:37:41.327"}]
            """;
        var endpoint = new GetCollateralHistoryNativeEndpoint(new FakeGetCollateralHistoryProtocolEndpoint((count, before, after) => Success("GetCollateralHistory", "GET", "/v1/me/getcollateralhistory", body)));

        var call = await endpoint.CallAsync(new GetCollateralHistoryRequest { Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal(4995L, call.Response![0].Id);
        Assert.Equal("JPY", call.Response[0].CurrencyCode);
        Assert.Equal(-6m, call.Response[0].Change);
        Assert.Equal("CLEARING_COLL", call.Response[0].ReasonCode);
        Assert.Equal(new DateTimeOffset(2017, 5, 18, 2, 37, 41, 327, TimeSpan.Zero), call.Response[0].Date);
    }

    [Fact]
    public async Task GetCollateralHistory_ReturnsSemantic_WhenCountIsInvalid()
    {
        var endpoint = new GetCollateralHistoryNativeEndpoint(new FakeGetCollateralHistoryProtocolEndpoint((count, before, after) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetCollateralHistoryRequest { Count = 0 });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetTradingCommission_MapsObject()
    {
        var body = """{"commission_rate":0.001}""";
        var endpoint = new GetTradingCommissionNativeEndpoint(new FakeGetTradingCommissionProtocolEndpoint(_ => Success("GetTradingCommission", "GET", "/v1/me/gettradingcommission", body)));

        var call = await endpoint.CallAsync(new GetTradingCommissionRequest { ProductCode = ProductCodes.BtcJpy });

        Assert.True(call.IsSuccess);
        Assert.NotNull(call.Response);
        Assert.Equal(0.001m, call.Response!.CommissionRate);
    }

    [Fact]
    public async Task GetTradingCommission_ReturnsSemantic_WhenProductCodeIsMissing()
    {
        var endpoint = new GetTradingCommissionNativeEndpoint(new FakeGetTradingCommissionProtocolEndpoint(_ => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetTradingCommissionRequest { ProductCode = "" });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
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
    public async Task SendChildOrder_ReturnsSemantic_WhenMarketPriceIsProvided()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Market,
            Side = OrderSides.Buy,
            Size = 0.01m,
            Price = 100m,
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendChildOrder_EncodesPrice_ForLimitRequest()
    {
        var fake = new FakeSendChildOrderProtocolEndpoint(_ => Success("SendChildOrder", "POST", "/v1/me/sendchildorder", """{"child_order_acceptance_id":"JRF123"}"""));
        var endpoint = new SendChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Limit,
            Side = OrderSides.Buy,
            Price = 5000000m,
            Size = 0.01m,
            MinuteToExpire = 1,
            TimeInForce = TimeInForces.Gtc,
        });

        Assert.True(call.IsSuccess);
        Assert.NotNull(fake.LastBodyJson);
        Assert.Contains("\"price\":5000000", fake.LastBodyJson!, StringComparison.Ordinal);
        Assert.Contains("\"minute_to_expire\":1", fake.LastBodyJson!, StringComparison.Ordinal);
        Assert.Contains("\"time_in_force\":\"GTC\"", fake.LastBodyJson!, StringComparison.Ordinal);
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
    public async Task CancelChildOrder_ReturnsSemantic_WhenBothIdentifiersAreProvided()
    {
        var fake = new FakeCancelChildOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new CancelChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderId = "JOR123",
            ChildOrderAcceptanceId = "JRF123",
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

    [Fact]
    public async Task CancelChildOrder_AllowsObjectBodyResponse()
    {
        var fake = new FakeCancelChildOrderProtocolEndpoint(_ => Success("CancelChildOrder", "POST", "/v1/me/cancelchildorder", "{}"));
        var endpoint = new CancelChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderAcceptanceId = "JRF123",
        });

        Assert.True(call.IsSuccess);
    }

    [Fact]
    public async Task CancelChildOrder_OmitsUnusedIdentifier()
    {
        var fake = new FakeCancelChildOrderProtocolEndpoint(_ => Success("CancelChildOrder", "POST", "/v1/me/cancelchildorder", string.Empty));
        var endpoint = new CancelChildOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderAcceptanceId = "JRF123",
        });

        Assert.True(call.IsSuccess);
        Assert.NotNull(fake.LastBodyJson);
        Assert.Contains("\"child_order_acceptance_id\":\"JRF123\"", fake.LastBodyJson!, StringComparison.Ordinal);
        Assert.DoesNotContain("child_order_id", fake.LastBodyJson!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CancelAllChildOrders_ReturnsSemantic_WhenProductCodeIsMissing()
    {
        var fake = new FakeCancelAllChildOrdersProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new CancelAllChildOrdersNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelAllChildOrdersRequest { ProductCode = "" });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CancelAllChildOrders_AllowsEmptyBodyResponse()
    {
        var fake = new FakeCancelAllChildOrdersProtocolEndpoint(_ => Success("CancelAllChildOrders", "POST", "/v1/me/cancelallchildorders", string.Empty));
        var endpoint = new CancelAllChildOrdersNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelAllChildOrdersRequest
        {
            ProductCode = ProductCodes.BtcJpy,
        });

        Assert.True(call.IsSuccess);
        Assert.NotNull(fake.LastBodyJson);
        Assert.Contains("product_code", fake.LastBodyJson!, StringComparison.Ordinal);
    }

    private static Call<ProtocolRequest, ProtocolResponse> Success(string endpointId, string method, string path, string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = endpointId, Method = method, Path = path, Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PrivateEndpointModule, EndpointId = endpointId, Scope = "Private", Auth = "KeySecret" });
    }
}
