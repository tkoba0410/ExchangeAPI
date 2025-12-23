using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet;

/// <summary>
/// bitFlyer 公開 REST API の Wire 実装。
/// </summary>
internal sealed class BitflyerPublicApi : IBitflyerPublicApi
{
    private readonly Raw.IBitflyerRawMarketDataApi _raw;

    public BitflyerPublicApi(Raw.IBitflyerRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public Task<Ticker> GetTickerRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapAsync(_raw.GetTickerAsync(productCode, useAliasPath, cancellationToken), BitflyerWireMapper.MapTicker);

    public Task<Board> GetBoardRawAsync(
        string productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapAsync(_raw.GetBoardAsync(productCode, useAliasPath, cancellationToken), BitflyerWireMapper.MapBoard);

    public Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsRawAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapListAsync(
            _raw.GetExecutionsAsync(productCode, count, before, after, useAliasPath, cancellationToken),
            BitflyerWireMapper.MapExecution);

    public Task<IReadOnlyList<Market>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapListAsync(_raw.GetMarketsAsync(region, useAliasPath, cancellationToken), BitflyerWireMapper.MapMarket);

    public Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default) =>
        MapListAsync(_raw.GetChatsAsync(fromDate, region, cancellationToken), BitflyerWireMapper.MapChat);

    public Task<HealthResponse> GetHealthAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        MapAsync(_raw.GetHealthAsync(productCode, cancellationToken), BitflyerWireMapper.MapHealth);

    public Task<BoardStateResponse> GetBoardStateAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        MapAsync(_raw.GetBoardStateAsync(productCode, cancellationToken), BitflyerWireMapper.MapBoardState);

    public Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default) =>
        MapAsync(_raw.GetCorporateLeverageAsync(cancellationToken), BitflyerWireMapper.MapCorporateLeverage);

    public Task<FundingRateResponse> GetFundingRateAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        MapAsync(_raw.GetFundingRateAsync(productCode, cancellationToken), BitflyerWireMapper.MapFundingRate);

    private static async Task<T> MapAsync<TSource, T>(Task<TSource> sourceTask, Func<TSource, T> map)
    {
        var source = await sourceTask.ConfigureAwait(false);
        return map(source);
    }

    private static async Task<IReadOnlyList<T>> MapListAsync<TSource, T>(
        Task<IReadOnlyList<TSource>> sourceTask,
        Func<TSource, T> map)
    {
        var source = await sourceTask.ConfigureAwait(false);
        return source.Select(map).ToArray();
    }
}
