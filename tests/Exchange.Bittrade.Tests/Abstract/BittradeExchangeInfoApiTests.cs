using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.Apis.ExchangeInfo;
using Common.Contract.Dtos;
using Common.Transport.Protocol;
using Common.Transport.Transport;
using Xunit;

namespace ExchangeApi.Adapter.Bittrade.Tests;

public class BittradeExchangeInfoApiTests
{
    [Fact]
    public async Task GetExchangeInfoAsync_MapsSymbols()
    {
        var json = """
        {
          "status": "ok",
          "data": [
            {
              "symbol": "btcjpy",
              "base-currency": "btc",
              "quote-currency": "jpy",
              "price-precision": 2,
              "amount-precision": 4,
              "min-order-amt": "0.0001",
              "min-order-value": "1000",
              "state": "online"
            }
          ]
        }
        """;

        var api = CreateApi("/v1/common/symbols", json);

        var info = await api.GetExchangeInfoAsync();

        Assert.Single(info.Markets);
        var m = info.Markets[0];
        Assert.Equal("BTC/JPY", m.Symbol);
        Assert.Equal("btcjpy", m.ProductCode);
        Assert.Equal(0.01m, m.PriceIncrement);
        Assert.Equal(0.0001m, m.SizeIncrement);
        Assert.Equal(0.0001m, m.MinSize);
        Assert.Equal(1000m, m.MinNotional);
        Assert.True(m.IsSupported);
    }

    private static BittradeExchangeInfoApi CreateApi(string expectedPath, string responseJson)
    {
        var handler = new StubHandler(expectedPath, responseJson);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var transport = new HttpTransport(client, disposeHttpClient: true);
        var restClient = new RestClient(client.BaseAddress!, transport);
        return new BittradeExchangeInfoApi(restClient);
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
