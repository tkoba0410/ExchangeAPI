using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Adapter.Bitflyer.Adapters;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer の ExchangeInfo 実装。現状は対応可否を返すスケルトン。
/// </summary>
public sealed class BitflyerExchangeInfoApi : IExchangeInfoApi
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);
    private static ExchangeInfo? _cached;
    private static DateTimeOffset _lastUpdated;

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_cached is { } cached && DateTimeOffset.UtcNow - _lastUpdated < CacheTtl)
        {
            return Task.FromResult(cached);
        }

        // 現状は REST 縦スライス対象の BTC/JPY のみを返す。
        var markets = new List<ExchangeMarketInfo>
        {
            // bitFlyer Lightning BTC/JPY: 最小数量 0.001 BTC, 価格単位 1 円, 数量刻み 0.001 BTC を初期値とする。
            new("BTC/JPY", "BTC_JPY", "Spot", MinSize: 0.001m, PriceIncrement: 1m, SizeIncrement: 0.001m),
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

        var info = BitflyerExchangeInfoMapper.MapExchangeInfo(markets, features, null);
        _cached = info;
        _lastUpdated = DateTimeOffset.UtcNow;
        return Task.FromResult(info);
    }
}
