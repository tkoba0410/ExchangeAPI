using System.Net;
using System.Net.Http;
using ExchangeApi.Stage10.Bitflyer.Composition.Factory;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Http;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Composition.Tests;

public sealed class BitflyerStage10ClientFactoryTests
{
    [Fact]
    public async Task CreateProtocolClient_WithoutCredentials_ExposesPublicOnly()
    {
        var transport = new RoutingTransport();
        using var bundle = BitflyerStage10ClientFactory.CreateProtocolClient(new BitflyerStage10ClientOptions
        {
            BaseUri = new Uri("https://example.com"),
            TransportConfig = new TransportConfig.ExternalTransport(transport),
        });

        var call = await bundle.Public.GetTickerAsync();
        var ok = Assert.IsType<CallResult<ExchangeApi.Transport.Wire.WireResponse>.Ok>(call.Result);

        Assert.Null(bundle.Private);
        Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)ok.Response.StatusCode);
        Assert.Equal("/v1/getticker", transport.LastRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task CreateNativeClient_WithCredentials_ExposesPrivateAndSharesProtocolRuntime()
    {
        var transport = new RoutingTransport();
        using var bundle = BitflyerStage10ClientFactory.CreateNativeClient(new BitflyerStage10ClientOptions
        {
            BaseUri = new Uri("https://example.com"),
            TransportConfig = new TransportConfig.ExternalTransport(transport),
            Credentials = new BitflyerApiCredentials
            {
                ApiKey = "dummy-key",
                ApiSecret = "dummy-secret",
            },
        });

        var publicCall = await bundle.Public.GetTickerAsync(new ExchangeApi.Stage10.Bitflyer.Native.Public.Requests.GetTickerRequest());
        var publicOk = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos.GetTickerResponse>.Ok>(publicCall.Result);

        var privateCall = await bundle.Private!.GetBalanceAsync(new ExchangeApi.Stage10.Bitflyer.Native.Private.Requests.GetBalanceRequest());
        var privateOk = Assert.IsType<CallResult<IReadOnlyList<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.GetBalance.Item>>.Ok>(privateCall.Result);

        Assert.NotNull(bundle.Protocol.Private);
        Assert.Equal(ProductCodes.BtcJpy, publicOk.Response.ProductCode);
        Assert.Single(privateOk.Response);
        Assert.Equal(2, transport.CallCount);
    }

    [Fact]
    public async Task CreateProtocolClient_WithTickerAliasEnabled_UsesAliasPathForTransport()
    {
        var transport = new RoutingTransport();
        using var bundle = BitflyerStage10ClientFactory.CreateProtocolClient(new BitflyerStage10ClientOptions
        {
            BaseUri = new Uri("https://example.com"),
            TransportConfig = new TransportConfig.ExternalTransport(transport),
            UseTickerAliasPath = true,
        });

        var call = await bundle.Public.GetTickerAsync();
        var ok = Assert.IsType<CallResult<ExchangeApi.Transport.Wire.WireResponse>.Ok>(call.Result);

        Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)ok.Response.StatusCode);
        Assert.Equal("/v1/getticker", call.Request.Path);
        Assert.Equal("/v1/ticker", transport.LastRequest!.RequestUri!.AbsolutePath);
    }

    private sealed class RoutingTransport : IHttpTransport
    {
        public int CallCount { get; private set; }

        public HttpRequestMessage? LastRequest { get; private set; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            CallCount++;
            LastRequest = request;

            var json = request.RequestUri!.AbsolutePath switch
            {
                "/v1/getticker" => """
                    {
                      "product_code": "BTC_JPY",
                      "state": "RUNNING",
                      "timestamp": "2024-01-01T00:00:00Z",
                      "tick_id": 1,
                      "best_bid": 100,
                      "best_ask": 101,
                      "best_bid_size": 0.1,
                      "best_ask_size": 0.2,
                      "total_bid_depth": 10,
                      "total_ask_depth": 20,
                      "market_bid_size": 1.2,
                      "market_ask_size": 1.3,
                      "ltp": 100.5,
                      "volume": 100,
                      "volume_by_product": 200
                    }
                    """,
                "/v1/me/getbalance" => """
                    [
                      { "currency_code": "JPY", "amount": 1000, "available": 900 }
                    ]
                    """,
                _ => "{}",
            };

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json),
            });
        }
    }
}
