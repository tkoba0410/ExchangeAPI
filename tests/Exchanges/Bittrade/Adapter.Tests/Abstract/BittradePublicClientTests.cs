using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Extensions;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradePublicClientTests
{
    [Fact]
    public async Task GetTimestampAsync_ReturnsRawResponse()
    {
        var json = """{ "status":"ok", "data": 1700000000000 }""";
        var client = CreateClient("/v1/common/timestamp", json);

        var call = await client.Raw<BittradeRawApi>().ExchangeInfo.GetTimestampAsync(new GetRawTimestampRequest());
        var response = Unwrap(call, "Bittrade.GetTimestamp");

        Assert.Equal("ok", response.Status);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1700000000000), response.Data);
    }

    [Fact]
    public async Task GetSymbolsAsync_ReturnsRawResponse()
    {
        var json = """
        {
          "status":"ok",
          "data":[
            {
              "symbol":"btcjpy",
              "base-currency":"btc",
              "quote-currency":"jpy",
              "price-precision": 2,
              "amount-precision": 6,
              "value-precision": 8,
              "min-order-amt":"0.001",
              "min-order-value":"1000",
              "state":"online"
            }
          ]
        }
        """;
        var client = CreateClient("/v1/common/symbols", json);

        var call = await client.Raw<BittradeRawApi>().ExchangeInfo.GetSymbolsAsync(new GetRawSymbolsRequest());
        var response = Unwrap(call, "Bittrade.GetSymbols");

        Assert.Equal("ok", response.Status);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data!);
        Assert.Equal("btcjpy", response.Data![0].Symbol);
    }

    private static TRes Unwrap<TReq, TRes>(Call<TReq, TRes> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TRes>.Ok ok => ok.Response,
            CallResult<TRes>.Err err => throw new InvalidOperationException(
                $"{operation} failed: {err.Error.Kind} {err.Error.Message}"),
            _ => throw new InvalidOperationException($"{operation} returned unknown call result.")
        };
    }

    private static BittradePublicClient CreateClient(string expectedPath, string responseJson)
    {
        var handler = new StubHandler(expectedPath, responseJson);
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var transport = new HttpTransport(http, disposeHttpClient: true);
        var restClient = new RestClient(http.BaseAddress!, transport);
        return new BittradePublicClient(restClient);
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
