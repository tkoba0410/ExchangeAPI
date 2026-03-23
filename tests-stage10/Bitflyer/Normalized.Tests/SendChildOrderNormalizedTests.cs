using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Normalized.Tests;

public sealed class SendChildOrderNormalizedTests
{
    [Fact]
    public async Task SendChildOrderAsync_WithLimitAndPrice_EncodesBodyAndMapsResponse()
    {
        var wire = new StubPrivateWireApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk(
                "SendChildOrder",
                """{ "child_order_acceptance_id": "JRF20240101-000000-000001" }"""));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
        });
        var ok = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.SendChildOrderResponse>.Ok>(call.Result);

        Assert.Equal("JRF20240101-000000-000001", ok.Response.ChildOrderAcceptanceId);
        Assert.Contains($"\"product_code\":\"{ProductCodes.BtcJpy}\"", wire.LastBodyJson);
        Assert.Contains("\"child_order_type\":\"LIMIT\"", wire.LastBodyJson);
        Assert.Contains("\"price\":100", wire.LastBodyJson);
        Assert.DoesNotContain("minute_to_expire", wire.LastBodyJson);
        Assert.DoesNotContain("time_in_force", wire.LastBodyJson);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithLimitWithoutPrice_ReturnsSemanticError()
    {
        var wire = new StubPrivateWireApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Null(wire.LastBodyJson);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithMarketAndPrice_ReturnsSemanticError()
    {
        var wire = new StubPrivateWireApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "MARKET",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithTooLargeMinuteToExpire_ReturnsSemanticError()
    {
        var wire = new StubPrivateWireApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
            MinuteToExpire = 43201,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Contains("43200", err.Error.Message);
        Assert.Null(wire.LastBodyJson);
    }

    [Fact]
    public async Task SendChildOrderAsync_WithNon200Status_ReturnsHttpError()
    {
        var wire = new StubPrivateWireApi(
            getBalanceCall: CreateWireOk("GetBalance", "[]"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""),
            sendChildOrderCall: CreateWireOk(
                "SendChildOrder",
                """{ "child_order_acceptance_id": "JRF20240101-000000-000001" }""",
                statusCode: 201));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.SendChildOrderAsync(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.SendChildOrderResponse>.Err>(call.Result);

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
            Meta: new CallMeta("Wire", "Transport", endpointId));

    private sealed class StubPrivateWireApi : IBitflyerPrivateWireApi
    {
        private readonly Call<WireCallSpec, WireResponse> _getBalanceCall;
        private readonly Call<WireCallSpec, WireResponse> _sendChildOrderCall;
        private readonly Call<WireCallSpec, WireResponse> _cancelChildOrderCall;

        public StubPrivateWireApi(
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
