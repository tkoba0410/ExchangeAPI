using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Http;
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

        var ticker = await api.GetTickerAsync(Symbol.BtcJpy);

        Assert.Equal(Symbol.BtcJpy, ticker.Symbol);
        Assert.Equal(100m, ticker.LastTradedPrice);
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

        var book = await api.GetOrderBookAsync(Symbol.BtcJpy);

        Assert.Equal(2, book.Bids.Count);
        Assert.Equal(2, book.Asks.Count);
        Assert.Equal(100m, book.Bids[0].Price);
        Assert.Equal(101m, book.Asks[0].Price);
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

        var executions = await api.GetMarketExecutionsAsync(Symbol.BtcJpy);

        Assert.Equal(2, executions.Count);
        Assert.Equal(Side.Buy, executions[0].Side);
        Assert.Equal(Side.Sell, executions[1].Side);
        Assert.Equal(Symbol.BtcJpy, executions[0].Symbol);
    }

    private static BittradeMarketDataApi CreateApi(string expectedPath, string responseJson)
    {
        var handler = new StubHandler(expectedPath, responseJson);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var transport = new HttpTransport(client, disposeHttpClient: true);
        var restClient = new RestClient(client.BaseAddress!, transport);
        return new BittradeMarketDataApi(restClient);
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
