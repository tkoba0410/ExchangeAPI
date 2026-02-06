using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Internal;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Http;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public class BittradeMarketApiTests
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

        var call = await api.GetExecutionsPublicAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<ExecutionsPublicResponse>.Ok>(call.Result);
        var executions = ok.Response.Items;

        Assert.Equal(2, executions.Count);
        Assert.Equal(Side.Buy, executions[0].Side);
        Assert.Equal(Side.Sell, executions[1].Side);
        Assert.Equal(new Symbol("BTC/JPY"), executions[0].Symbol);
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
        var markets = CreateResolver(new ExchangeMarketInfo(Symbol.ParseOrThrow("BTC/JPY"), ProductCode.ParseOrThrow("btcjpy"), MarketType.ParseOrThrow("Spot")));
        var wireTransport = new ExchangeApi.Transport.Wire.WireTransport(restClient);
        var wire = new BittradeWireCallExecutor(wireTransport);
        var raw = new ExchangeApi.Exchanges.Bittrade.Api.Raw.Api.BittradeRawApi(wire);
        var normalizedMarketData = new ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api.BittradeNormalizedPublicApi(raw);
        return new MarketApi(normalizedMarketData, markets);
    }

    private static IExchangeMarketResolver CreateResolver(params ExchangeMarketInfo[] markets) =>
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfoDto(markets, null, null, null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoProvider
    {
        private readonly ExchangeInfoDto _info;

        public StubExchangeInfoApi(ExchangeInfoDto info) => _info = info;

        public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
            CancellationToken cancellationToken = default)
        {
            var meta = CallMeta.CreateInternal("Contracts", "StubExchangeInfoApi");
            var call = new Call<ExchangeInfoRequest, ExchangeInfoDto>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: new ExchangeInfoRequest(),
                Result: new CallResult<ExchangeInfoDto>.Ok(_info),
                Meta: meta);
            return Task.FromResult(call);
        }

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
