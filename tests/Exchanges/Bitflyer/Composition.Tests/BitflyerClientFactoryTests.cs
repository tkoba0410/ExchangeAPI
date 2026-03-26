using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using System.Net;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Composition.Tests;

public sealed class BitflyerClientFactoryTests
{
    [Fact]
    public void CreateProtocolClient_WithoutCredentials_HasOnlyPublic()
    {
        var bundle = BitflyerClientFactory.CreateProtocolClient();

        Assert.NotNull(bundle.Public);
        Assert.Null(bundle.Private);
    }

    [Fact]
    public void CreateProtocolClient_WithCredentials_HasPrivate()
    {
        var bundle = BitflyerClientFactory.CreateProtocolClient(new BitflyerClientOptions
        {
            Credentials = new BitflyerApiCredentials
            {
                ApiKey = "key",
                ApiSecret = "secret",
            },
        });

        Assert.NotNull(bundle.Public);
        Assert.NotNull(bundle.Private);
    }

    [Fact]
    public void CreateNativeClient_WithCredentials_WiresProtocolAndNative()
    {
        var bundle = BitflyerClientFactory.CreateNativeClient(new BitflyerClientOptions
        {
            Credentials = new BitflyerApiCredentials
            {
                ApiKey = "key",
                ApiSecret = "secret",
            },
        });

        Assert.NotNull(bundle.Public);
        Assert.NotNull(bundle.Private);
        Assert.NotNull(bundle.Protocol.Public);
        Assert.NotNull(bundle.Protocol.Private);
    }

    [Fact]
    public async Task CreateProtocolClient_WithExternalHttpClient_DoesNotDisposeCallerClient()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var bundle = BitflyerClientFactory.CreateProtocolClient(httpClient);

        bundle.Dispose();
        bundle.Dispose();

        using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.com/health"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task CreateNativeClient_WithExternalHttpClient_DoesNotDisposeCallerClient_WhenNestedProtocolIsDisposed()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var bundle = BitflyerClientFactory.CreateNativeClient(httpClient);

        bundle.Protocol.Dispose();
        bundle.Dispose();
        bundle.Dispose();

        using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.com/health"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok"),
            });
        }
    }
}
