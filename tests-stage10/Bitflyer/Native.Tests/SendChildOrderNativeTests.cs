using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Native.Tests;

public sealed class SendChildOrderNativeTests
{
    [Fact]
    public async Task SendChildOrderAsync_WithLimitAndPrice_EncodesBodyAndMapsResponse()
    {
        var protocol = new StubPrivateProtocolApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk(
                "SendChildOrder",
                """{ "child_order_acceptance_id": "JRF20240101-000000-000001" }"""));
        var api = new BitflyerPrivateNativeApi(protocol);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
        });
        var ok = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.SendChildOrderResponse>.Ok>(call.Result);

        Assert.Equal("JRF20240101-000000-000001", ok.Response.ChildOrderAcceptanceId);
        Assert.Contains($"\"product_code\":\"{ProductCodes.BtcJpy}\"", protocol.LastBodyJson);
        Assert.Contains("\"child_order_type\":\"LIMIT\"", protocol.LastBodyJson);
        Assert.Contains("\"price\":100", protocol.LastBodyJson);
        Assert.DoesNotContain("minute_to_expire", protocol.LastBodyJson);
        Assert.DoesNotContain("time_in_force", protocol.LastBodyJson);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithLimitWithoutPrice_ReturnsSemanticError()
    {
        var protocol = new StubPrivateProtocolApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"));
        var api = new BitflyerPrivateNativeApi(protocol);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Null(protocol.LastBodyJson);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithMarketAndPrice_ReturnsSemanticError()
    {
        var protocol = new StubPrivateProtocolApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"));
        var api = new BitflyerPrivateNativeApi(protocol);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "MARKET",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithTooLargeMinuteToExpire_ReturnsSemanticError()
    {
        var protocol = new StubPrivateProtocolApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"));
        var api = new BitflyerPrivateNativeApi(protocol);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
            MinuteToExpire = 43201,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Contains("43200", err.Error.Message);
        Assert.Null(protocol.LastBodyJson);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithNon200Status_ReturnsHttpError()
    {
        var protocol = new StubPrivateProtocolApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk(
                "SendChildOrder",
                """{ "child_order_acceptance_id": "JRF20240101-000000-000001" }""",
                statusCode: 201));
        var api = new BitflyerPrivateNativeApi(protocol);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        Assert.Equal(201, err.Error.HttpStatus);
    }

    private static Call<WireCallSpec, WireResponse> CreateWireOk(string endpointId, string json, int statusCode = 200) =>
        new(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: new WireCallSpec("POST", "/test", endpointId),
            Result: new CallResult<WireResponse>.Ok(new WireResponse(statusCode, json)),
            Meta: new CallMeta("Protocol", "Transport", endpointId));

    private sealed class StubPrivateProtocolApi : IBitflyerPrivateProtocolApi
    {
        private readonly Call<WireCallSpec, WireResponse> _getBalanceCall;
        private readonly Call<WireCallSpec, WireResponse> _sendChildOrderCall;
        private readonly Call<WireCallSpec, WireResponse> _cancelChildOrderCall;

        public StubPrivateProtocolApi(
            Call<WireCallSpec, WireResponse> getBalanceCall,
            Call<WireCallSpec, WireResponse> cancelChildOrderCall,
            Call<WireCallSpec, WireResponse> sendChildOrderCall)
        {
            _getBalanceCall = getBalanceCall;
            _cancelChildOrderCall = cancelChildOrderCall;
            _sendChildOrderCall = sendChildOrderCall;
        }

        public string? LastBodyJson { get; private set; }

        public Task<Call<WireCallSpec, WireResponse>> GetBalanceAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_getBalanceCall);

        public Task<Call<WireCallSpec, WireResponse>> SendChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default)
        {
            LastBodyJson = bodyJson;
            return Task.FromResult(_sendChildOrderCall);
        }

        public Task<Call<WireCallSpec, WireResponse>> CancelChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_cancelChildOrderCall);
    }
}
