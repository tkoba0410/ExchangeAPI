using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Common.Application.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Http;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public class MarketApiTests
{
    [Fact]
    public async Task GetDetailMergedCallAsync_MapsMergedResponse()
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

        var call = await api.GetDetailMergedCallAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<TickerResponse>.Ok>(call.Result);
        var ticker = ok.Response;

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.Equal(new Price(100m), ticker.LastTradedPrice);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1700000000000), ticker.Timestamp);
    }

    [Fact]
    public async Task GetDepthCallAsync_MapsDepth()
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

        var call = await api.GetDepthCallAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<BoardResponse>.Ok>(call.Result);
        var book = ok.Response;

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

        var call = await api.GetExecutionsPublicAsync(new ExecutionsPublicRequest(new Symbol("BTC/JPY")));
        var ok = Assert.IsType<CallResult<ExecutionsPublicResponse>.Ok>(call.Result);
        var executions = ok.Response.Items;

        Assert.Equal(2, executions.Count);
        Assert.Equal(Side.Buy, executions[0].Side);
        Assert.Equal(Side.Sell, executions[1].Side);
        Assert.Equal(new Symbol("BTC/JPY"), executions[0].Symbol);
    }

    [Fact]
    public async Task GetCandlesticksAsync_MapsKlines()
    {
        var json = """
        { "status":"ok", "ts":1700000000000,
          "data": [
            { "id": 1700000000, "open": 90, "close": 100, "low": 80, "high": 110, "amount": 1.2, "vol": 1200000, "count": 10 }
          ]
        }
        """;
        var api = CreateApi("/market/history/kline?period=1min&symbol=btcjpy&size=1", json);

        var call = await api.GetCandlesticksAsync(new CandlesticksRequest(new Symbol("BTC/JPY"), new Period("1min"), Size: 1));
        var ok = Assert.IsType<CallResult<CandlesticksResponse>.Ok>(call.Result);
        var item = Assert.Single(ok.Response.Items);

        Assert.Equal(TimeSpan.FromMinutes(1), item.Timescale);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), item.OpenTime);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000).AddMinutes(1), item.CloseTime);
    }

    [Fact]
    public async Task GetDetailMergedCallAsync_UnknownSymbol_Throws()
    {
        var api = CreateApi("/market/detail/merged?symbol=btcjpy", "{}");

        var call = await api.GetDetailMergedCallAsync(new Symbol("DOGE/JPY"));
        var err = Assert.IsType<CallResult<TickerResponse>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }

    private static MarketApi CreateApi(string expectedPath, string responseJson)
    {
        var handler = new StubHandler(expectedPath, responseJson);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var transport = new HttpTransport(client, disposeHttpClient: true);
        var restClient = new RestClient(client.BaseAddress!, transport);
        var markets = CreateResolver();
        var wireTransport = new ExchangeApi.Transport.Wire.WireTransport(restClient);
        var wire = new WireCallExecutor(wireTransport);
        var raw = new ExchangeApi.Exchanges.Bittrade.Raw.Api.RawApi(wire);
        var normalizedMarketData = new ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api.NormalizedPublicApi(raw);
        return new MarketApi(normalizedMarketData, markets);
    }

    private static IExchangeMarketResolver CreateResolver() =>
        new BittradeMarketCatalogResolver();

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
