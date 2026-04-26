using System.Net;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class BitflyerProtocolTransportTests
{
    [Fact]
    public async Task SendAsync_ResolvesAbsoluteRequestUri_FromBaseUri()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var transport = new BitflyerProtocolTransport(
            httpClient,
            new Uri("https://api.bitflyer.com"),
            new NoOpProtocolDebugLogger(),
            apiCredentialProvider: null);

        var result = await transport.SendAsync(new ProtocolRequest
        {
            EndpointId = "GetMarkets",
            Method = "GET",
            Path = "/v1/getmarkets",
            Query = new Dictionary<string, string>
            {
                ["product_code"] = "BTC_JPY",
            },
        }, ProtocolTransportAuthMode.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new Uri("https://api.bitflyer.com/v1/getmarkets?product_code=BTC_JPY"), handler.LastRequestUri);
    }

    [Fact]
    public async Task SendAsync_WithRequestTimeout_ReturnsTransportTimeout()
    {
        var handler = new PendingHandler();
        using var httpClient = new HttpClient(handler);
        var requestTimeout = TimeSpan.FromMilliseconds(50);
        var transport = new BitflyerProtocolTransport(
            httpClient,
            new Uri("https://api.bitflyer.com"),
            new NoOpProtocolDebugLogger(),
            apiCredentialProvider: null,
            requestTimeout);

        var result = await transport.SendAsync(new ProtocolRequest
        {
            EndpointId = "GetMarkets",
            Method = "GET",
            Path = "/v1/getmarkets",
        }, ProtocolTransportAuthMode.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CallErrorKinds.Transport, result.Error!.Kind);
        Assert.Equal($"Request timed out after {requestTimeout:c}.", result.Error.Message);
    }

    [Fact]
    public async Task SendAsync_WhenCredentialProviderFails_ReturnsSanitizedTransportError()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var transport = new BitflyerProtocolTransport(
            httpClient,
            new Uri("https://api.bitflyer.com"),
            new NoOpProtocolDebugLogger(),
            new FailingCredentialProvider());

        var result = await transport.SendAsync(new ProtocolRequest
        {
            EndpointId = "GetBalance",
            Method = "GET",
            Path = "/v1/me/getbalance",
        }, ProtocolTransportAuthMode.KeySecret);

        Assert.False(result.IsSuccess);
        Assert.Equal(CallErrorKinds.Transport, result.Error!.Kind);
        Assert.Equal(nameof(ApiCredentialErrorKind.DecryptFailed), result.Error.VenueErrorCode);
        Assert.Contains("credential source could not be opened", result.Error.Message);
        Assert.DoesNotContain("api-key", result.Error.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("api-secret", result.Error.Message, StringComparison.Ordinal);
        Assert.Null(handler.LastRequestUri);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]"),
            });
        }
    }

    private sealed class PendingHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

    private sealed class FailingCredentialProvider : IApiCredentialProvider
    {
        public ValueTask<IApiCredentialSession> OpenSessionAsync(CancellationToken cancellationToken = default)
        {
            throw new ApiCredentialException(
                ApiCredentialErrorKind.DecryptFailed,
                "credential source could not be opened");
        }
    }
}
