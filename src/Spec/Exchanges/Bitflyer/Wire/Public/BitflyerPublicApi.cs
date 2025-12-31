using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Converters;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
using WirePublic = ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

/// <summary>
/// bitFlyer 公開 REST API の Wire 実装。
/// </summary>
internal sealed class BitflyerPublicApi : IBitflyerPublicApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;
    private readonly Raw.IBitflyerRawMarketDataApi _raw;

    public BitflyerPublicApi(Raw.IBitflyerRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public Task<WireResponse<Ticker>> GetTickerRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapAsync<Raw.Ticker, WirePublic.Ticker>(
            _raw.GetTickerAsync(productCode, useAliasPath, cancellationToken),
            BitflyerWireMapper.MapTicker);

    public Task<WireResponse<Board>> GetBoardRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapAsync<Raw.Board, WirePublic.Board>(
            _raw.GetBoardAsync(productCode, useAliasPath, cancellationToken),
            BitflyerWireMapper.MapBoard);

    public Task<WireResponse<IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsRawAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapListAsync<Raw.ExecutionPublicResponse, WirePublic.ExecutionPublicResponse>(
            _raw.GetExecutionsAsync(productCode, count, before, after, useAliasPath, cancellationToken),
            BitflyerWireMapper.MapExecution);

    public Task<WireResponse<IReadOnlyList<Market>>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        MapListAsync<Raw.Market, WirePublic.Market>(
            _raw.GetMarketsAsync(region, useAliasPath, cancellationToken),
            BitflyerWireMapper.MapMarket);

    public Task<WireResponse<IReadOnlyList<Chat>>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default) =>
        MapListAsync<Raw.Chat, WirePublic.Chat>(
            _raw.GetChatsAsync(fromDate, region, cancellationToken),
            BitflyerWireMapper.MapChat);

    public Task<WireResponse<HealthResponse>> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        MapAsync<Raw.HealthResponse, WirePublic.HealthResponse>(
            _raw.GetHealthAsync(productCode, cancellationToken),
            BitflyerWireMapper.MapHealth);

    public Task<WireResponse<BoardStateResponse>> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        MapAsync<Raw.BoardStateResponse, WirePublic.BoardStateResponse>(
            _raw.GetBoardStateAsync(productCode, cancellationToken),
            BitflyerWireMapper.MapBoardState);

    public Task<WireResponse<CorporateLeverageResponse>> GetCorporateLeverageAsync(CancellationToken cancellationToken = default) =>
        MapAsync<Raw.CorporateLeverageResponse, WirePublic.CorporateLeverageResponse>(
            _raw.GetCorporateLeverageAsync(cancellationToken),
            BitflyerWireMapper.MapCorporateLeverage);

    public Task<WireResponse<FundingRateResponse>> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        MapAsync<Raw.FundingRateResponse, WirePublic.FundingRateResponse>(
            _raw.GetFundingRateAsync(productCode, cancellationToken),
            BitflyerWireMapper.MapFundingRate);

    private static async Task<WireResponse<T>> MapAsync<TSource, T>(Task<TSource> sourceTask, Func<TSource, T> map)
    {
        var source = await sourceTask.ConfigureAwait(false);
        return new WireResponse<T>(Exchange, map(source));
    }

    private static async Task<WireResponse<IReadOnlyList<T>>> MapListAsync<TSource, T>(
        Task<IReadOnlyList<TSource>> sourceTask,
        Func<TSource, T> map)
    {
        var source = await sourceTask.ConfigureAwait(false);
        return new WireResponse<IReadOnlyList<T>>(Exchange, source.Select(map).ToArray());
    }
}
