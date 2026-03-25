using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class AdditionalPrivateNativeEndpointTests
{
    [Fact]
    public async Task GetPermissions_MapsTopLevelArray()
    {
        var endpoint = new GetPermissionsNativeEndpoint(
            new FakeGetPermissionsProtocolEndpoint(Success("GetPermissions", "GET", "/v1/me/getpermissions", """["Read","Trade"]""")));

        var call = await endpoint.CallAsync(new GetPermissionsRequest());

        Assert.True(call.IsSuccess);
        Assert.Equal(["Read", "Trade"], call.Response);
    }

    [Fact]
    public async Task GetAddresses_MapsTopLevelArray()
    {
        var body = """[{"type":"NORMAL","currency_code":"BTC","address":"1abc"}]""";
        var endpoint = new GetAddressesNativeEndpoint(
            new FakeGetAddressesProtocolEndpoint(Success("GetAddresses", "GET", "/v1/me/getaddresses", body)));

        var call = await endpoint.CallAsync(new GetAddressesRequest());

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("NORMAL", call.Response![0].Type);
        Assert.Equal("BTC", call.Response[0].CurrencyCode);
        Assert.Equal("1abc", call.Response[0].Address);
    }

    [Fact]
    public async Task GetCoinIns_MapsTopLevelArray()
    {
        var body = """
            [{"id":1,"order_id":"ORD1","currency_code":"BTC","amount":0.1,"address":"1abc","tx_hash":"tx1","status":"COMPLETED","event_date":"2024-01-02T03:04:05.678"}]
            """;
        var endpoint = new GetCoinInsNativeEndpoint(
            new FakeGetCoinInsProtocolEndpoint((count, before, after) => Success("GetCoinIns", "GET", "/v1/me/getcoinins", body)));

        var call = await endpoint.CallAsync(new GetCoinInsRequest { Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("ORD1", call.Response![0].OrderId);
        Assert.Equal("BTC", call.Response[0].CurrencyCode);
        Assert.Equal(0.1m, call.Response[0].Amount);
    }

    [Fact]
    public async Task GetCoinIns_ReturnsSemantic_WhenCountIsInvalid()
    {
        var endpoint = new GetCoinInsNativeEndpoint(
            new FakeGetCoinInsProtocolEndpoint((count, before, after) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetCoinInsRequest { Count = 0 });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetCoinOuts_MapsTopLevelArray()
    {
        var body = """
            [{"id":1,"order_id":"ORD1","currency_code":"BTC","amount":0.1,"address":"1abc","tx_hash":"tx1","fee":0.0004,"additional_fee":0.0001,"status":"COMPLETED","event_date":"2024-01-02T03:04:05.678"}]
            """;
        var endpoint = new GetCoinOutsNativeEndpoint(
            new FakeGetCoinOutsProtocolEndpoint((count, before, after) => Success("GetCoinOuts", "GET", "/v1/me/getcoinouts", body)));

        var call = await endpoint.CallAsync(new GetCoinOutsRequest { Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal(0.0004m, call.Response![0].Fee);
        Assert.Equal(0.0001m, call.Response[0].AdditionalFee);
    }

    [Fact]
    public async Task GetBankAccounts_MapsTopLevelArray()
    {
        var body = """
            [{"id":1,"is_verified":true,"bank_name":"ABC","branch_name":"DEF","account_type":"SAVINGS","account_number":"1234567","account_name":"TARO"}]
            """;
        var endpoint = new GetBankAccountsNativeEndpoint(
            new FakeGetBankAccountsProtocolEndpoint(Success("GetBankAccounts", "GET", "/v1/me/getbankaccounts", body)));

        var call = await endpoint.CallAsync(new GetBankAccountsRequest());

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.True(call.Response![0].IsVerified);
        Assert.Equal("ABC", call.Response[0].BankName);
    }

    [Fact]
    public async Task GetDeposits_MapsTopLevelArray()
    {
        var body = """
            [{"id":1,"order_id":"ORD1","currency_code":"JPY","amount":1000,"status":"COMPLETED","event_date":"2024-01-02T03:04:05.678"}]
            """;
        var endpoint = new GetDepositsNativeEndpoint(
            new FakeGetDepositsProtocolEndpoint((count, before, after) => Success("GetDeposits", "GET", "/v1/me/getdeposits", body)));

        var call = await endpoint.CallAsync(new GetDepositsRequest { Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("JPY", call.Response![0].CurrencyCode);
        Assert.Equal(1000m, call.Response[0].Amount);
    }

    [Fact]
    public async Task Withdraw_MapsMessageId_AndSerializesBody()
    {
        var fake = new FakeWithdrawProtocolEndpoint(_ => Success("Withdraw", "POST", "/v1/me/withdraw", """{"message_id":"MSG1"}"""));
        var endpoint = new WithdrawNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new WithdrawRequest
        {
            CurrencyCode = "JPY",
            BankAccountId = 1,
            Amount = 1000m,
            Code = "123456",
        });

        Assert.True(call.IsSuccess);
        Assert.Equal("MSG1", call.Response!.MessageId);

        using var document = JsonDocument.Parse(fake.LastBodyJson!);
        var root = document.RootElement;
        Assert.Equal("JPY", root.GetProperty("currency_code").GetString());
        Assert.Equal(1L, root.GetProperty("bank_account_id").GetInt64());
        Assert.Equal(1000m, root.GetProperty("amount").GetDecimal());
        Assert.Equal("123456", root.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Withdraw_ReturnsSemantic_WhenVenueReturnsNegativeStatus()
    {
        var fake = new FakeWithdrawProtocolEndpoint(_ => Success("Withdraw", "POST", "/v1/me/withdraw", """{"status":-700,"error_message":"Invalid code."}"""));
        var endpoint = new WithdrawNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new WithdrawRequest
        {
            CurrencyCode = "JPY",
            BankAccountId = 1,
            Amount = 1000m,
            Code = "123456",
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
        Assert.Equal("Invalid code.", call.Error.Message);
    }

    [Fact]
    public async Task GetWithdrawals_MapsTopLevelArray()
    {
        var body = """
            [{"id":1,"order_id":"ORD1","currency_code":"JPY","amount":1000,"status":"COMPLETED","event_date":"2024-01-02T03:04:05.678"}]
            """;
        var endpoint = new GetWithdrawalsNativeEndpoint(
            new FakeGetWithdrawalsProtocolEndpoint((count, before, after, messageId) => Success("GetWithdrawals", "GET", "/v1/me/getwithdrawals", body)));

        var call = await endpoint.CallAsync(new GetWithdrawalsRequest { Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("ORD1", call.Response![0].OrderId);
        Assert.Equal(1000m, call.Response[0].Amount);
    }

    [Fact]
    public async Task GetWithdrawals_ReturnsSemantic_WhenMessageIdIsBlank()
    {
        var endpoint = new GetWithdrawalsNativeEndpoint(
            new FakeGetWithdrawalsProtocolEndpoint((count, before, after, messageId) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetWithdrawalsRequest { MessageId = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetParentOrders_MapsTopLevelArray()
    {
        var body = """
            [{"id":1,"parent_order_id":"JPO1","product_code":"BTC_JPY","side":"BUY","parent_order_type":"SIMPLE","price":30000,"average_price":30000,"size":0.1,"parent_order_state":"COMPLETED","expire_date":"2024-01-02T03:04:05.678","parent_order_date":"2024-01-02T03:04:05.678","parent_order_acceptance_id":"JPA1","outstanding_size":0,"cancel_size":0,"executed_size":0.1,"total_commission":0}]
            """;
        var endpoint = new GetParentOrdersNativeEndpoint(
            new FakeGetParentOrdersProtocolEndpoint((productCode, count, before, after, parentOrderState) => Success("GetParentOrders", "GET", "/v1/me/getparentorders", body)));

        var call = await endpoint.CallAsync(new GetParentOrdersRequest { ProductCode = ProductCodes.BtcJpy, Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("JPO1", call.Response![0].ParentOrderId);
        Assert.Equal("COMPLETED", call.Response[0].ParentOrderState);
    }

    [Fact]
    public async Task GetParentOrders_ReturnsSemantic_WhenParentOrderStateIsInvalid()
    {
        var endpoint = new GetParentOrdersNativeEndpoint(
            new FakeGetParentOrdersProtocolEndpoint((productCode, count, before, after, parentOrderState) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetParentOrdersRequest { ParentOrderState = "QUEUED" });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetParentOrder_MapsObject()
    {
        var body = """
            {"id":1,"parent_order_id":"JPO1","order_method":"SIMPLE","expire_date":"2024-01-02T03:04:05.678","time_in_force":"GTC","parameters":[{"product_code":"BTC_JPY","condition_type":"LIMIT","side":"BUY","price":30000,"size":0.1,"trigger_price":0,"offset":0}],"parent_order_acceptance_id":"JPA1"}
            """;
        var endpoint = new GetParentOrderNativeEndpoint(
            new FakeGetParentOrderProtocolEndpoint((parentOrderId, parentOrderAcceptanceId) => Success("GetParentOrder", "GET", "/v1/me/getparentorder", body)));

        var call = await endpoint.CallAsync(new GetParentOrderRequest { ParentOrderId = "JPO1" });

        Assert.True(call.IsSuccess);
        Assert.NotNull(call.Response);
        Assert.Equal("JPO1", call.Response!.ParentOrderId);
        Assert.Single(call.Response.Parameters);
        Assert.Equal("LIMIT", call.Response.Parameters[0].ConditionType);
        Assert.Equal(0m, call.Response.Parameters[0].TriggerPrice);
        Assert.Equal(0m, call.Response.Parameters[0].Offset);
    }

    [Fact]
    public async Task GetParentOrder_ReturnsSemantic_WhenIdentifiersAreInvalid()
    {
        var endpoint = new GetParentOrderNativeEndpoint(
            new FakeGetParentOrderProtocolEndpoint((parentOrderId, parentOrderAcceptanceId) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetParentOrderRequest());

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetParentOrder_ReturnsSemantic_WhenBothIdentifiersAreProvided()
    {
        var endpoint = new GetParentOrderNativeEndpoint(
            new FakeGetParentOrderProtocolEndpoint((parentOrderId, parentOrderAcceptanceId) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetParentOrderRequest
        {
            ParentOrderId = "JPO1",
            ParentOrderAcceptanceId = "JPA1",
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task GetBalanceHistory_MapsTopLevelArray()
    {
        var body = """
            [{"id":1,"trade_date":"2024-01-02T03:04:05.678","event_date":"2024-01-02T03:04:05.678","product_code":"BTC_JPY","currency_code":"JPY","trade_type":"BUY","price":30000,"amount":1000,"quantity":0.1,"commission":0.1,"balance":99999,"order_id":"ORD1"}]
            """;
        var endpoint = new GetBalanceHistoryNativeEndpoint(
            new FakeGetBalanceHistoryProtocolEndpoint((currencyCode, count, before, after) => Success("GetBalanceHistory", "GET", "/v1/me/getbalancehistory", body)));

        var call = await endpoint.CallAsync(new GetBalanceHistoryRequest { CurrencyCode = "JPY", Count = 10 });

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("JPY", call.Response![0].CurrencyCode);
        Assert.Equal("ORD1", call.Response[0].OrderId);
        Assert.Equal(new DateTimeOffset(2024, 1, 1, 18, 4, 5, 678, TimeSpan.Zero), call.Response[0].TradeDate);
        Assert.Equal(new DateTimeOffset(2024, 1, 2, 3, 4, 5, 678, TimeSpan.Zero), call.Response[0].EventDate);
    }

    [Fact]
    public async Task GetBalanceHistory_ReturnsSemantic_WhenCurrencyCodeIsBlank()
    {
        var endpoint = new GetBalanceHistoryNativeEndpoint(
            new FakeGetBalanceHistoryProtocolEndpoint((currencyCode, count, before, after) => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetBalanceHistoryRequest { CurrencyCode = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendParentOrder_OmitsOptionalFields_AndMapsResponse()
    {
        var fake = new FakeSendParentOrderProtocolEndpoint(_ => Success("SendParentOrder", "POST", "/v1/me/sendparentorder", """{"parent_order_acceptance_id":"JPA1"}"""));
        var endpoint = new SendParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendParentOrderRequest
        {
            OrderMethod = ParentOrderMethods.Simple,
            MinuteToExpire = null,
            TimeInForce = null,
            Parameters =
            [
                new SendParentOrderParameter
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ConditionType = ParentOrderConditionTypes.Limit,
                    Side = OrderSides.Buy,
                    Price = 30000m,
                    Size = 0.1m,
                    TriggerPrice = null,
                    Offset = null,
                },
            ],
        });

        Assert.True(call.IsSuccess);
        Assert.Equal("JPA1", call.Response!.ParentOrderAcceptanceId);

        using var document = JsonDocument.Parse(fake.LastBodyJson!);
        var root = document.RootElement;
        Assert.Equal("SIMPLE", root.GetProperty("order_method").GetString());
        Assert.False(root.TryGetProperty("minute_to_expire", out _));
        Assert.False(root.TryGetProperty("time_in_force", out _));

        var parameter = root.GetProperty("parameters")[0];
        Assert.Equal("BTC_JPY", parameter.GetProperty("product_code").GetString());
        Assert.Equal("LIMIT", parameter.GetProperty("condition_type").GetString());
        Assert.True(parameter.TryGetProperty("price", out _));
        Assert.False(parameter.TryGetProperty("trigger_price", out _));
        Assert.False(parameter.TryGetProperty("offset", out _));
    }

    [Fact]
    public async Task SendParentOrder_ReturnsSemantic_WhenParameterCountDoesNotMatchMethod()
    {
        var fake = new FakeSendParentOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendParentOrderRequest
        {
            OrderMethod = ParentOrderMethods.Oco,
            Parameters =
            [
                new SendParentOrderParameter
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ConditionType = ParentOrderConditionTypes.Market,
                    Side = OrderSides.Buy,
                    Size = 0.1m,
                },
            ],
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendParentOrder_ReturnsSemantic_WhenLimitParameterOmitsPrice()
    {
        var fake = new FakeSendParentOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendParentOrderRequest
        {
            OrderMethod = ParentOrderMethods.Simple,
            Parameters =
            [
                new SendParentOrderParameter
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ConditionType = ParentOrderConditionTypes.Limit,
                    Side = OrderSides.Buy,
                    Size = 0.1m,
                },
            ],
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task SendParentOrder_ReturnsSemantic_WhenMarketParameterIncludesPrice()
    {
        var fake = new FakeSendParentOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new SendParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new SendParentOrderRequest
        {
            OrderMethod = ParentOrderMethods.Simple,
            Parameters =
            [
                new SendParentOrderParameter
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ConditionType = ParentOrderConditionTypes.Market,
                    Side = OrderSides.Buy,
                    Price = 30000m,
                    Size = 0.1m,
                },
            ],
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CancelParentOrder_AllowsEmptyBodyResponse()
    {
        var fake = new FakeCancelParentOrderProtocolEndpoint(_ => Success("CancelParentOrder", "POST", "/v1/me/cancelparentorder", string.Empty));
        var endpoint = new CancelParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelParentOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ParentOrderId = "JPO1",
        });

        Assert.True(call.IsSuccess);
        Assert.NotNull(fake.LastBodyJson);
    }

    [Fact]
    public async Task CancelParentOrder_OmitsUnusedIdentifier()
    {
        var fake = new FakeCancelParentOrderProtocolEndpoint(_ => Success("CancelParentOrder", "POST", "/v1/me/cancelparentorder", string.Empty));
        var endpoint = new CancelParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelParentOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ParentOrderAcceptanceId = "JPA1",
        });

        Assert.True(call.IsSuccess);
        Assert.NotNull(fake.LastBodyJson);
        Assert.Contains("\"parent_order_acceptance_id\":\"JPA1\"", fake.LastBodyJson!, StringComparison.Ordinal);
        Assert.DoesNotContain("parent_order_id", fake.LastBodyJson!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CancelParentOrder_ReturnsSemantic_WhenIdentifiersAreInvalid()
    {
        var fake = new FakeCancelParentOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new CancelParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelParentOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CancelParentOrder_ReturnsSemantic_WhenBothIdentifiersAreProvided()
    {
        var fake = new FakeCancelParentOrderProtocolEndpoint(_ => throw new InvalidOperationException());
        var endpoint = new CancelParentOrderNativeEndpoint(fake);

        var call = await endpoint.CallAsync(new CancelParentOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ParentOrderId = "JPO1",
            ParentOrderAcceptanceId = "JPA1",
        });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    private static Call<ProtocolRequest, ProtocolResponse> Success(string endpointId, string method, string path, string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = endpointId, Method = method, Path = path, Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PrivateEndpointModule, EndpointId = endpointId, Scope = "Private", Auth = "KeySecret" });
    }
}
