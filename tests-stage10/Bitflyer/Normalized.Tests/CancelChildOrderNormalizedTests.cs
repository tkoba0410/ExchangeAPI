using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Normalized.Tests;

public sealed class CancelChildOrderNormalizedTests
{
    [Fact]
    public async Task CancelChildOrderAsync_WithAcceptanceId_EncodesBodyAndAcceptsEmptySuccess()
    {
        var wire = new StubPrivateWireApi(
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", string.Empty));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.CancelChildOrderAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderAcceptanceId = "JRF20240101-000000-000001",
        });
        var ok = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.CancelChildOrderResponse>.Ok>(call.Result);

        Assert.NotNull(ok.Response);
        Assert.Contains($"\"product_code\":\"{ProductCodes.BtcJpy}\"", wire.LastCancelBodyJson);
        Assert.Contains("\"child_order_acceptance_id\":\"JRF20240101-000000-000001\"", wire.LastCancelBodyJson);
        Assert.DoesNotContain("child_order_id", wire.LastCancelBodyJson);
    }

    [Fact]
    public async Task CancelChildOrderAsync_WithChildOrderId_AcceptsEmptyObjectSuccess()
    {
        var wire = new StubPrivateWireApi(
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", "{}"));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.CancelChildOrderAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderId = "JOR20150707-055555-022222",
        });
        var ok = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.CancelChildOrderResponse>.Ok>(call.Result);

        Assert.NotNull(ok.Response);
        Assert.Contains("\"child_order_id\":\"JOR20150707-055555-022222\"", wire.LastCancelBodyJson);
        Assert.DoesNotContain("child_order_acceptance_id", wire.LastCancelBodyJson);
    }

    [Fact]
    public async Task CancelChildOrderAsync_WithBothIds_ReturnsSemanticError()
    {
        var wire = new StubPrivateWireApi(
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", string.Empty));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.CancelChildOrderAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderId = "JOR-1",
            ChildOrderAcceptanceId = "JRF-1",
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.CancelChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Null(wire.LastCancelBodyJson);
    }

    [Fact]
    public async Task CancelChildOrderAsync_WithNeitherId_ReturnsSemanticError()
    {
        var wire = new StubPrivateWireApi(
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", string.Empty));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.CancelChildOrderAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.CancelChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Null(wire.LastCancelBodyJson);
    }

    [Fact]
    public async Task CancelChildOrderAsync_WithNon200Status_ReturnsHttpError()
    {
        var wire = new StubPrivateWireApi(
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", string.Empty, statusCode: 201));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.CancelChildOrderAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderAcceptanceId = "JRF-1",
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.CancelChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        Assert.Equal(201, err.Error.HttpStatus);
    }

    [Fact]
    public async Task CancelChildOrderAsync_WithNonObjectJson_ReturnsCodecError()
    {
        var wire = new StubPrivateWireApi(
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", "[]"));
        var api = new BitflyerPrivateNormalizedApi(wire);

        var call = await api.CancelChildOrderAsync(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderAcceptanceId = "JRF-1",
        });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos.CancelChildOrderResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Codec, err.Error.Kind);
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
        private readonly Call<WireCallSpec, WireResponse> _cancelChildOrderCall;

        public StubPrivateWireApi(Call<WireCallSpec, WireResponse> cancelChildOrderCall)
        {
            _cancelChildOrderCall = cancelChildOrderCall;
        }

        public string? LastCancelBodyJson { get; private set; }

        public Task<Call<WireCallSpec, WireResponse>> GetBalanceAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateWireOk("GetBalance", "[]"));

        public Task<Call<WireCallSpec, WireResponse>> SendChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateWireOk("SendChildOrder", "{}"));

        public Task<Call<WireCallSpec, WireResponse>> CancelChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default)
        {
            LastCancelBodyJson = bodyJson;
            return Task.FromResult(_cancelChildOrderCall);
        }
    }
}
