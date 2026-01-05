using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Spec.CallCommon;
using ExchangeInfoDto = ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.ExchangeInfo;

/// <summary>
/// bitFlyer の ExchangeInfo 実装。現状は対応可否を返すスケルトン。
/// </summary>
public sealed class BitflyerExchangeInfoApi : IExchangeInfoApi
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan DailyMaintenanceEndJst = new(4, 10, 0);
    private static ExchangeInfoDto? _cached;
    private static DateTimeOffset _lastUpdated;

    public Task<ExchangeInfoDto> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_cached is { } cached && DateTimeOffset.UtcNow - _lastUpdated < CacheTtl)
        {
            return Task.FromResult(cached);
        }

        // 現状は REST 縦スライス対象の BTC/JPY のみを返す。
        var markets = new List<ExchangeMarketInfo>
        {
            // bitFlyer Lightning BTC/JPY: 最小数量 0.001 BTC, 価格単位 1 円, 数量刻み 0.001 BTC を初期値とする。
            new("BTC/JPY", "BTC_JPY", "Spot", MinSize: new Size(0.001m), PriceIncrement: new Price(1m), SizeIncrement: new Size(0.001m), FeeCurrency: "BTC"),
        };

        var features = new ExchangeFeatureFlags(
            SupportsWebSocket: false,
            SupportsMargin: true,
            SupportsStopOrder: true,
            SupportsParentOrder: true,
            SupportsCandlestick: false,
            SupportsOrderBookDelta: false,
            SupportsRealtimeExecutions: false,
            SupportsWithdraw: false);

        var maintenance = new ExchangeMaintenance(
            Status: ExchangeMaintenanceStatus.Planned,
            PlannedUntil: GetNextDailyMaintenanceEndUtc(),
            Message: "Daily maintenance 04:00-04:10 JST");

        var info = BitflyerExchangeInfoMapper.MapExchangeInfo(markets, features, null, maintenance);
        _cached = info;
        _lastUpdated = DateTimeOffset.UtcNow;
        return Task.FromResult(info);
    }

    public async Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetExchangeInfoRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var info = await GetExchangeInfoAsync(cancellationToken).ConfigureAwait(false);
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: BitflyerOperations.ExchangeInfo.GetExchangeInfo,
                Tags: null,
                Children: null);
            return new Call<GetExchangeInfoRequest, ExchangeInfoDto>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: DateTimeOffset.UtcNow - startedAt,
                Request: request,
                Result: new CallResult<ExchangeInfoDto>.Ok(info),
                Meta: meta);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetExchangeInfoRequest, ExchangeInfoDto>(
                request,
                startedAt,
                BitflyerOperations.ExchangeInfo.GetExchangeInfo,
                ex);
        }
    }

    private static DateTimeOffset? GetNextDailyMaintenanceEndUtc()
    {
        // bitFlyer は毎日 04:00-04:10 (JST) に定期メンテ。終了予定のみを返す。
        try
        {
            var jst = GetTokyoTimeZone();
            var nowJst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, jst);
            var todayEnd = nowJst.Date.Add(DailyMaintenanceEndJst);
            var nextEndLocal = nowJst.TimeOfDay < DailyMaintenanceEndJst
                ? todayEnd
                : todayEnd.AddDays(1);
            var nextEndUtc = TimeZoneInfo.ConvertTimeToUtc(nextEndLocal, jst);
            return new DateTimeOffset(nextEndUtc);
        }
        catch
        {
            // タイムゾーン解決が失敗した場合はメンテ終了時刻なしで返す。
            return null;
        }
    }

    private static TimeZoneInfo GetTokyoTimeZone()
    {
        // Linux は "Asia/Tokyo", Windows は "Tokyo Standard Time"。
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        }
    }
}
