using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Common.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using Xunit;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class ErrorEnrichTests
{
    [Fact]
    public async Task GetDetailMergedCallAsync_EnrichesExchangeAndOperation()
    {
        var api = new MarketApi(new NormalizedPublicApi(new ThrowingRawApi()), CreateResolver());

        var call = await api.GetDetailMergedCallAsync(new Symbol("BTC/JPY"), CancellationToken.None);

        var err = Assert.IsType<CallResult<TickerResponse>.Err>(call.Result);
        Assert.Equal("Bittrade.MarketData.GetTicker", call.Meta.Component);
        Assert.Equal("boom", err.Error.Message);
    }

    private sealed class ThrowingRawApi : RawApiStub
    {
        public override Task<Call<RawPublicRequests.GetDetailMergedRequest, RawPublicDtos.GetDetailMergedResponse>> GetDetailMergedCallAsync(
            RawPublicRequests.GetDetailMergedRequest request,
            CancellationToken cancellationToken = default) =>
            throw new ExchangeApiException("boom");

        public override Task<Call<RawPublicRequests.GetDepthRequest, RawPublicDtos.GetDepthResponse>> GetDepthCallAsync(
            RawPublicRequests.GetDepthRequest request,
            CancellationToken cancellationToken = default) =>
            throw new ExchangeApiException("boom");

        public override Task<Call<RawPublicRequests.GetTradeRequest, RawPublicDtos.GetTradeResponse>> GetTradeCallAsync(
            RawPublicRequests.GetTradeRequest request,
            CancellationToken cancellationToken = default) =>
            throw new ExchangeApiException("boom");

        public override Task<Call<RawPublicRequests.GetHistoryKlineRequest, RawPublicDtos.GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
            RawPublicRequests.GetHistoryKlineRequest request,
            CancellationToken cancellationToken = default) =>
            throw new ExchangeApiException("boom");

        public override Task<Call<RawPublicRequests.GetTickersRequest, RawPublicDtos.GetTickersResponse>> GetTickersCallAsync(
            RawPublicRequests.GetTickersRequest request,
            CancellationToken cancellationToken = default) =>
            throw new ExchangeApiException("boom");

        public override Task<Call<RawPublicRequests.GetHistoryTradeRequest, RawPublicDtos.GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
            RawPublicRequests.GetHistoryTradeRequest request,
            CancellationToken cancellationToken = default) =>
            throw new ExchangeApiException("boom");
    }

    private static IExchangeMarketResolver CreateResolver() =>
        new BittradeMarketCatalogResolver();
}
