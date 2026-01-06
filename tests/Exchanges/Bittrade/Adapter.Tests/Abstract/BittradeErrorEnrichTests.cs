using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Domain.Services;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using ContractsRequests = ExchangeApi.Contracts.Requests;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Spec.CallCommon;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeErrorEnrichTests
{
    [Fact]
    public async Task GetTickerAsync_EnrichesExchangeAndOperation()
    {
        var api = new BittradeMarketDataApi(new ThrowingMarketDataApi(), CreateResolver());

        var call = await api.GetTickerCallAsync(new Symbol("BTC/JPY"), CancellationToken.None);

        var err = Assert.IsType<CallResult<Ticker>.Err>(call.Result);
        Assert.Equal("Bittrade.Market.GetTicker", call.Meta.Component);
        Assert.Equal("boom", err.Error.Message);
    }

    private sealed class ThrowingMarketDataApi : IBittradeNormalizedMarketDataApi
    {
        public Task<Call<NormalizedRequests.GetTickerRequest, BittradeTickerNormalized>> GetTickerCallAsync(
            string symbol,
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");

        public Task<Call<NormalizedRequests.GetOrderBookRequest, BittradeOrderBookNormalized>> GetOrderBookCallAsync(
            string symbol,
            ExchangeApi.Exchanges.Bittrade.Normalize.Types.BittradeDepthType? depthType = null,
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");

        public Task<Call<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetExecutionsCallAsync(
            string symbol,
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");
    }

    private static IExchangeMarketResolver CreateResolver() =>
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfo(
            new[] { new ExchangeMarketInfo("BTC/JPY", "btcjpy", "Spot") },
            null,
            null,
            null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoApi
    {
        private readonly ExchangeInfo _info;

        public StubExchangeInfoApi(ExchangeInfo info) => _info = info;

        public Task<Call<ContractsRequests.GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new Call<ContractsRequests.GetExchangeInfoRequest, ExchangeInfo>(
                CallId.New(),
                DateTimeOffset.UtcNow,
                TimeSpan.Zero,
                new ContractsRequests.GetExchangeInfoRequest(),
                new CallResult<ExchangeInfo>.Ok(_info),
                new CallMeta("Contracts", "StubExchangeInfo", null, null)));
    }
}
