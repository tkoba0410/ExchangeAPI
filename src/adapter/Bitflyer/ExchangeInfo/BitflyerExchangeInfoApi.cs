using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer の ExchangeInfo 実装。現状は対応可否を返すスケルトン。
/// </summary>
public sealed class BitflyerExchangeInfoApi : IExchangeInfoApi
{
    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
    {
        // 現状は固定値のスケルトン。必要に応じてマーケット一覧を拡張する。
        var markets = new List<ExchangeMarketInfo>
        {
            new("BTC/JPY", "BTC_JPY", "Spot", MinSize: null, PriceIncrement: null, SizeIncrement: null),
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

        var info = new ExchangeInfo(markets, features, null);
        return Task.FromResult(info);
    }
}
