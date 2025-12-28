using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Domain.Services;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Http;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public class BittradeMarketDataApiTests
{
    [Fact]
    public async Task GetTickerAsync_MapsMergedResponse()
    {
        var json = """
        { "status":"ok", "ts":1700000000000,
          "tick": {
            "close": 100,
            "open": 90,
            "low": 80,
            "high": 110,
            "amount": 1.2,
            "vol": 1200000,
            "ts": 1700000000000,
            "bid": [99, 0.5],
            "ask": [101, 0.4]
          }
        }
        """;
        var api = CreateApi("/market/detail/merged?symbol=btcjpy", json);

        var ticker = await api.GetTickerAsync(new Symbol("BTC/JPY"));

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.Equal(new Price(100m), ticker.LastTradedPrice);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1700000000000), ticker.Timestamp);
    }

    [Fact]
    public async Task GetOrderBookAsync_MapsDepth()
    {
        var json = """
        { "status":"ok",
          "tick": {
            "bids": [[100, 0.1], [99, 0.2]],
            "asks": [[101, 0.3], [102, 0.4]]
          }
        }
        """;
        var api = CreateApi("/market/depth?symbol=btcjpy&type=step0", json);

        var book = await api.GetOrderBookAsync(new Symbol("BTC/JPY"));

        Assert.Equal(2, book.Bids.Count);
        Assert.Equal(2, book.Asks.Count);
        Assert.Equal(new Price(100m), book.Bids[0].Price);
        Assert.Equal(new Price(101m), book.Asks[0].Price);
    }

    [Fact]
    public async Task GetMarketExecutionsAsync_MapsTrades()
    {
        var json = """
        { "status":"ok",
          "tick": {
            "data": [
              { "id": 1, "price": 100, "amount": 0.1, "direction": "buy", "ts": 1700000000001 },
              { "id": 2, "price": 101, "amount": 0.2, "direction": "sell", "ts": 1700000000002 }
            ]
          }
        }
        """;
        var api = CreateApi("/market/trade?symbol=btcjpy", json);

        var executions = await api.GetMarketExecutionsAsync(new Symbol("BTC/JPY"));

        Assert.Equal(2, executions.Count);
        Assert.Equal(Side.Buy, executions[0].Side);
        Assert.Equal(Side.Sell, executions[1].Side);
        Assert.Equal(new Symbol("BTC/JPY"), executions[0].Symbol);
    }

    [Fact]
    public async Task GetTickerAsync_UnknownSymbol_Throws()
    {
        var api = CreateApi("/market/detail/merged?symbol=btcjpy", "{}");

        await Assert.ThrowsAsync<SymbolNotSupportedException>(() => api.GetTickerAsync(new Symbol("DOGE/JPY")));
    }

    private static BittradeMarketDataApi CreateApi(string expectedPath, string responseJson)
    {
        var handler = new StubHandler(expectedPath, responseJson);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var transport = new HttpTransport(client, disposeHttpClient: true);
        var restClient = new RestClient(client.BaseAddress!, transport);
        var markets = CreateResolver(new ExchangeMarketInfo("BTC/JPY", "btcjpy", "Spot"));
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient);
        return new BittradeMarketDataApi(normalizeBundle.MarketData, markets);
    }

    private static IExchangeMarketResolver CreateResolver(params ExchangeMarketInfo[] markets) =>
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfo(markets, null, null, null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoApi
    {
        private readonly ExchangeInfo _info;

        public StubExchangeInfoApi(ExchangeInfo info) => _info = info;

        public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_info);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly string _expectedPath;
        private readonly string _response;

        public StubHandler(string expectedPath, string response)
        {
            _expectedPath = expectedPath;
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.Equals(request.RequestUri?.PathAndQuery, _expectedPath, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var msg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_response)
            };
            return Task.FromResult(msg);
        }
    }
}
