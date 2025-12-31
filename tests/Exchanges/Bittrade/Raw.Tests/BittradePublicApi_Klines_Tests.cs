using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradePublicApi_Klines_Tests
{
    [Fact]
    public async Task GetKlinesAsync_UsesExpectedPath()
    {
        var fakeRest = new FakeRestClient();
        var api = new BittradeWireMarketDataApi(fakeRest);

        await api.GetKlinesAsync("BTC/JPY", "1day", size: 2, ct: CancellationToken.None);

        Assert.Equal("market/history/kline?period=1day&symbol=btcjpy&size=2", fakeRest.LastPath);
    }

    private sealed class FakeRestClient : IRestClient
    {
        public string? LastPath { get; private set; }

        public Task<TResponse> GetAsync<TResponse>(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        {
            LastPath = path;
            return Task.FromResult(default(TResponse)!);
        }

        public Task<HttpResponseMeta> GetRawAsync(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        {
            LastPath = path;
            return Task.FromResult(new HttpResponseMeta(200, Headers: null, Body: "{}"));
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<HttpResponseMeta> PostRawAsync<TRequest>(string path, TRequest body, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
