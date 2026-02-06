using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.GetExchangeInfoResponse;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ContractsRequests = ExchangeApi.Contracts.Facade.Requests;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using Xunit;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeErrorEnrichTests
{
    [Fact]
    public async Task GetDetailMergedCallAsync_EnrichesExchangeAndOperation()
    {
        var api = new MarketApi(new BittradeNormalizedPublicApi(new ThrowingRawApi()), CreateResolver());

        var call = await api.GetDetailMergedCallAsync(new Symbol("BTC/JPY"), CancellationToken.None);

        var err = Assert.IsType<CallResult<GetTickerResponse>.Err>(call.Result);
        Assert.Equal("Bittrade.Market.GetTicker", call.Meta.Component);
        Assert.Equal("boom", err.Error.Message);
    }

    private sealed class ThrowingRawApi : BittradeRawApiStub
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
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfoDto(
            new[] { new ExchangeMarketInfo(Symbol.ParseOrThrow("BTC/JPY"), ProductCode.ParseOrThrow("btcjpy"), MarketType.ParseOrThrow("Spot")) },
            null,
            null,
            null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoProvider
    {
        private readonly ExchangeInfoDto _info;

        public StubExchangeInfoApi(ExchangeInfoDto info) => _info = info;

        public Task<Call<ContractsRequests.GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new Call<ContractsRequests.GetExchangeInfoRequest, ExchangeInfoDto>(
                CallId.New(),
                DateTimeOffset.UtcNow,
                TimeSpan.Zero,
                new ContractsRequests.GetExchangeInfoRequest(),
                new CallResult<ExchangeInfoDto>.Ok(_info),
                CallMeta.CreateInternal("Contracts", "StubExchangeInfo")));

    }
}
