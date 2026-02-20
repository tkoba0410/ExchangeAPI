using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Composition;
using ExchangeApi.Exchanges.Bittrade.Composition;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Http;
using Xunit;

namespace ExchangeApi.Tests.Composition.Tests.Transport;

public sealed class CompositionTransportConfig_Tests
{
    [Fact]
    public async Task BitflyerFactory_PublicClient_UsesExternalTransportConfig()
    {
        var transport = new StaticResponseTransport(
            """
            {
              "product_code": "BTC_JPY",
              "timestamp": "2024-01-01T00:00:00Z",
              "tick_id": 123,
              "best_bid": 100,
              "best_ask": 101,
              "best_bid_size": 0.1,
              "best_ask_size": 0.2,
              "total_bid_depth": 10,
              "total_ask_depth": 20,
              "ltp": 100.5,
              "volume": 123.45,
              "volume_by_product": 200
            }
            """);

        var client = BitflyerFactory.CreateContractPublicClient(new BitflyerFactoryOptions
        {
            BaseUri = new Uri("https://example.com"),
            TransportConfig = new TransportConfig.ExternalTransport(transport),
        });

        var call = await client.GetTickerAsync(new TickerRequest(new Symbol("BTC/JPY")));
        var ok = Assert.IsType<CallResult<TickerResponse>.Ok>(call.Result);

        Assert.Equal(new Symbol("BTC/JPY"), ok.Response.Symbol);
        Assert.Equal(1, transport.CallCount);
    }

    [Fact]
    public async Task BittradeFactory_PublicClient_UsesExternalTransportConfig()
    {
        var transport = new StaticResponseTransport(
            """
            {
              "status": "ok",
              "ts": 1700000000000,
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
            """);

        var client = BittradeFactory.CreateContractPublicClient(new BittradeFactoryOptions
        {
            BaseUri = new Uri("https://example.com"),
            TransportConfig = new TransportConfig.ExternalTransport(transport),
        });

        var call = await client.GetTickerAsync(new TickerRequest(new Symbol("BTC/JPY")));
        var ok = Assert.IsType<CallResult<TickerResponse>.Ok>(call.Result);

        Assert.Equal(new Symbol("BTC/JPY"), ok.Response.Symbol);
        Assert.Equal(1, transport.CallCount);
    }

    private sealed class StaticResponseTransport : IHttpTransport
    {
        private readonly string _json;

        public StaticResponseTransport(string json)
        {
            _json = json;
        }

        public int CallCount { get; private set; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json),
            });
        }
    }
}
