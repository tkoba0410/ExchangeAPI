using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradePublicApi_Klines_Tests
{
    [Fact]
    public async Task GetKlinesAsync_UsesExpectedPath()
    {
        var fakeRest = new FakeRestClient();
        var api = new BittradePublicApi(fakeRest);

        await api.GetKlinesAsync(new Symbol("BTC/JPY"), "1day", size: 2, cancellationToken: CancellationToken.None);

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

        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
