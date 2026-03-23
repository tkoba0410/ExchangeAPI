using ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Wire.Tests;

public sealed class PrivateWireApiTests
{
    [Fact]
    public async Task GetBalanceAsync_BuildsCanonicalRequest()
    {
        var transport = new CaptureWireTransport();
        var api = new BitflyerPrivateWireApi(transport);

        var call = await api.GetBalanceAsync();

        Assert.Equal("GET", call.Request.Method);
        Assert.Equal("/v1/me/getbalance", call.Request.Path);
        Assert.Equal("GetBalance", call.Request.EndpointId);
        Assert.Null(call.Request.Query);
        Assert.Null(call.Request.BodyJson);
        Assert.Null(call.Request.Headers);
    }

    [Fact]
    public async Task SendChildOrderAsync_BuildsCanonicalRequest()
    {
        const string bodyJson = "{\"product_code\":\"BTC_JPY\"}";
        var transport = new CaptureWireTransport();
        var api = new BitflyerPrivateWireApi(transport);

        var call = await api.SendChildOrderAsync(bodyJson);

        Assert.Equal("POST", call.Request.Method);
        Assert.Equal("/v1/me/sendchildorder", call.Request.Path);
        Assert.Equal("SendChildOrder", call.Request.EndpointId);
        Assert.Null(call.Request.Query);
        Assert.Equal(bodyJson, call.Request.BodyJson);
        Assert.Null(call.Request.Headers);
    }

    [Fact]
    public async Task CancelChildOrderAsync_BuildsCanonicalRequest()
    {
        const string bodyJson = "{\"product_code\":\"BTC_JPY\",\"child_order_acceptance_id\":\"JRF-1\"}";
        var transport = new CaptureWireTransport();
        var api = new BitflyerPrivateWireApi(transport);

        var call = await api.CancelChildOrderAsync(bodyJson);

        Assert.Equal("POST", call.Request.Method);
        Assert.Equal("/v1/me/cancelchildorder", call.Request.Path);
        Assert.Equal("CancelChildOrder", call.Request.EndpointId);
        Assert.Null(call.Request.Query);
        Assert.Equal(bodyJson, call.Request.BodyJson);
        Assert.Null(call.Request.Headers);
    }

    private sealed class CaptureWireTransport : IWireTransport
    {
        public Task<Call<WireCallSpec, WireResponse>> SendAsync(
            WireCallSpec request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new Call<WireCallSpec, WireResponse>(
                    Id: CallId.New(),
                    StartedAt: DateTimeOffset.UtcNow,
                    Duration: TimeSpan.Zero,
                    Request: request,
                    Result: new CallResult<WireResponse>.Ok(new WireResponse(200, "{}")),
                    Meta: new CallMeta("Wire", "Transport", request.EndpointId)));
        }
    }
}
