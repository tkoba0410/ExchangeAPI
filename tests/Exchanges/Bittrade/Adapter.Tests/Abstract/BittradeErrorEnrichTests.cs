using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeErrorEnrichTests
{
    [Fact]
    public async Task GetTickerAsync_EnrichesExchangeAndOperation()
    {
        var rest = new ThrowingRestClient();
        var api = new BittradeMarketDataApi(rest);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() =>
            api.GetTickerAsync(new Symbol("BTC/JPY"), CancellationToken.None));

        Assert.Equal(ExchangeApi.Common.Enums.ExchangeCode.Bittrade, ex.Exchange);
        Assert.Equal("Bittrade.Market.GetTicker", ex.Operation);
    }

    private sealed class ThrowingRestClient : IRestClient
    {
        public Task<TResponse> GetAsync<TResponse>(
            string path,
            IReadOnlyDictionary<string, string?>? query = null,
            CancellationToken cancellationToken = default)
        {
            throw new ExchangeApiException("boom");
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(
            string path,
            TRequest body,
            CancellationToken cancellationToken = default)
        {
            throw new ExchangeApiException("boom");
        }
    }
}
