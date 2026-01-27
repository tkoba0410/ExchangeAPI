using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ContractsRequests = ExchangeApi.Contracts.Facade.Requests;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeErrorEnrichTests
{
    [Fact]
    public async Task GetDetailMergedCallAsync_EnrichesExchangeAndOperation()
    {
        var api = new MarketApi(new ThrowingMarketDataApi(), CreateResolver());

        var call = await api.GetDetailMergedCallAsync(new Symbol("BTC/JPY"), CancellationToken.None);

        var err = Assert.IsType<CallResult<Ticker>.Err>(call.Result);
        Assert.Equal("Bittrade.Market.GetTicker", call.Meta.Component);
        Assert.Equal("boom", err.Error.Message);
    }

    private sealed class ThrowingMarketDataApi : IBittradeNormalizedMarketDataApi
    {
        public Task<Call<NormalizedRequests.GetTickerRequest, BittradeTickerNormalized>> GetDetailMergedCallAsync(
            string productCode,
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");

        public Task<Call<NormalizedRequests.GetOrderBookRequest, BittradeOrderBookNormalized>> GetDepthCallAsync(
            string productCode,
            BittradeDepthType? depthType = null,
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");

        public Task<Call<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetTradeCallAsync(
            string productCode,
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");

        public Task<Call<NormalizedRequests.GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>> GetHistoryKlineCallAsync(
            string productCode,
            string period,
            int? size = null,
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");

        public Task<Call<NormalizedRequests.GetTickersRequest, IReadOnlyList<BittradeTickerEntryNormalized>>> GetTickersCallAsync(
            CancellationToken ct = default) =>
            throw new ExchangeApiException("boom");

        public Task<Call<NormalizedRequests.GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetHistoryTradeCallAsync(
            string productCode,
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
                CallMeta.CreateInternal("Contracts", "StubExchangeInfo")));

        public Task<Call<ContractsRequests.GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
            CancellationToken cancellationToken = default)
        {
            var request = new ContractsRequests.GetCurrencysRequest();
            return Task.FromResult(NotSupportedCall.Create<ContractsRequests.GetCurrencysRequest, IReadOnlyList<string>>(
                "Contracts",
                "StubExchangeInfo",
                request,
                "Currencys"));
        }

        public Task<Call<ContractsRequests.GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
            CancellationToken cancellationToken = default)
        {
            var request = new ContractsRequests.GetTimestampRequest();
            return Task.FromResult(NotSupportedCall.Create<ContractsRequests.GetTimestampRequest, DateTimeOffset>(
                "Contracts",
                "StubExchangeInfo",
                request,
                "Timestamp"));
        }
    }
}
