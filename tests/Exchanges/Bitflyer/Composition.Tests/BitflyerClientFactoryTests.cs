using System.Net;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Composition.Tests;

public sealed class BitflyerClientFactoryTests
{
    [Fact]
    public void CreateProtocolClient_WithoutCredentials_HasOnlyPublic()
    {
        var bundle = BitflyerClientFactory.CreateProtocolClientBundle();

        Assert.NotNull(bundle.Public);
        Assert.Null(bundle.Private);
    }

    [Fact]
    public void CreateProtocolClient_WithCredentials_HasPrivate()
    {
        var bundle = BitflyerClientFactory.CreateProtocolClientBundle(new BitflyerClientOptions
        {
            ApiCredentialProvider = new FakeCredentialProvider(),
        });

        Assert.NotNull(bundle.Public);
        Assert.NotNull(bundle.Private);
    }

    [Fact]
    public void CreateNativeClient_WithCredentials_WiresProtocolAndNative()
    {
        var bundle = BitflyerClientFactory.CreateNativeClientBundle(new BitflyerClientOptions
        {
            ApiCredentialProvider = new FakeCredentialProvider(),
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
        var bundle = BitflyerClientFactory.CreateProtocolClientBundle(httpClient);

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
        var bundle = BitflyerClientFactory.CreateNativeClientBundle(httpClient);

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

    private sealed class FakeCredentialProvider : IApiCredentialProvider
    {
        public ValueTask<IApiCredentialSession> OpenSessionAsync(CancellationToken cancellationToken = default)
        {
            return ValueTask.FromResult<IApiCredentialSession>(new FakeCredentialSession());
        }
    }

    private sealed class FakeCredentialSession : IApiCredentialSession
    {
        public string ApiKey => "key";

        public string Sign(string payload)
        {
            return "signature";
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
