using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Transport.Models;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Protocol.Tests;

public sealed class BitflyerProtocolTransportTests
{
    [Fact]
    public async Task SendAsync_WithTickerAliasEnabled_SendsAliasPathButKeepsCanonicalRequest()
    {
        var restClient = new CaptureRestClient();
        var transport = new BitflyerProtocolTransport(restClient, useTickerAliasPath: true);
        var request = new WireCallSpec(
            Method: "GET",
            Path: "/v1/getticker",
            EndpointId: "GetTicker",
            Query: "product_code=BTC_JPY");

        var call = await transport.SendAsync(request);

        Assert.Equal("/v1/getticker", call.Request.Path);
        Assert.Equal("/v1/ticker", restClient.LastPath);
        Assert.Equal("product_code=BTC_JPY", restClient.LastQuery);
    }

    [Fact]
    public async Task SendAsync_WithTickerAliasDisabled_SendsCanonicalPath()
    {
        var restClient = new CaptureRestClient();
        var transport = new BitflyerProtocolTransport(restClient, useTickerAliasPath: false);
        var request = new WireCallSpec(
            Method: "GET",
            Path: "/v1/getticker",
            EndpointId: "GetTicker");

        await transport.SendAsync(request);

        Assert.Equal("/v1/getticker", restClient.LastPath);
    }

    private sealed class CaptureRestClient : IRestClient
    {
        public string? LastPath { get; private set; }

        public string? LastQuery { get; private set; }

        public void Dispose()
        {
        }

        public Task<HttpResponseMeta> GetRawAsync(
            string path,
            IReadOnlyDictionary<string, string?>? query = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<HttpResponseMeta> SendRawAsync(
            string method,
            string path,
            string? query = null,
            string? bodyJson = null,
            IReadOnlyDictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            LastPath = path;
            LastQuery = query;

            return Task.FromResult(new HttpResponseMeta(
                StatusCode: 200,
                Headers: null,
                Body: "{}"));
        }
    }
}
