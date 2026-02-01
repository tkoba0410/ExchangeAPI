using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Compose;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Dynamic;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Static;

namespace ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Compose;

internal static class BittradeExchangeInfoComposer
{
    public static BittradeStaticExchangeInfo Compose(
        BittradeStaticExchangeInfo @static,
        BittradeDynamicExchangeInfo? dynamic)
    {
        if (@static is null) throw new ArgumentNullException(nameof(@static));
        if (dynamic is null) return @static;

        var markets = MergeMarkets(@static.Markets, dynamic.Markets);
        var features = MergeFeatures(@static.Features, dynamic.Features);
        var rateLimits = MergeRateLimits(@static.RateLimits, dynamic.RateLimits);
        var maintenance = MergeMaintenance(@static.Maintenance, dynamic.Maintenance);

        return new BittradeStaticExchangeInfo
        {
            Markets = markets,
            Features = features,
            RateLimits = rateLimits,
            Maintenance = maintenance
        };
    }

    private static IReadOnlyList<BittradeStaticMarketInfo> MergeMarkets(
        IReadOnlyList<BittradeStaticMarketInfo> staticMarkets,
        IReadOnlyList<BittradeDynamicMarketInfo>? dynamicMarkets)
    {
        var dynamic = (dynamicMarkets ?? Array.Empty<BittradeDynamicMarketInfo>())
            .Where(m => !string.IsNullOrWhiteSpace(m.ProductCode))
            .ToList();

        return ExchangeInfoMerge.MergeByKey(
            staticMarkets,
            dynamic,
            staticItem => staticItem.ProductCode,
            dynamicItem => dynamicItem.ProductCode ?? string.Empty,
            MergeMarket,
            dynamicItem => new BittradeStaticMarketInfo
            {
                Symbol = dynamicItem.Symbol ?? dynamicItem.ProductCode ?? string.Empty,
                ProductCode = dynamicItem.ProductCode ?? string.Empty,
                Type = dynamicItem.Type ?? "Spot",
                MinSize = dynamicItem.MinSize,
                MaxSize = dynamicItem.MaxSize,
                MinNotional = dynamicItem.MinNotional,
                PriceIncrement = dynamicItem.PriceIncrement,
                SizeIncrement = dynamicItem.SizeIncrement,
                MakerFeeRate = dynamicItem.MakerFeeRate,
                TakerFeeRate = dynamicItem.TakerFeeRate,
                FeeCurrency = dynamicItem.FeeCurrency,
                FeeType = dynamicItem.FeeType,
                IsSupported = dynamicItem.IsSupported,
                StatusNote = dynamicItem.StatusNote
            },
            StringComparer.OrdinalIgnoreCase);
    }

    private static BittradeStaticMarketInfo MergeMarket(BittradeStaticMarketInfo s, BittradeDynamicMarketInfo d) =>
        new()
        {
            Symbol = d.Symbol ?? s.Symbol,
            ProductCode = s.ProductCode,
            Type = d.Type ?? s.Type,
            MinSize = d.MinSize ?? s.MinSize,
            MaxSize = d.MaxSize ?? s.MaxSize,
            MinNotional = d.MinNotional ?? s.MinNotional,
            PriceIncrement = d.PriceIncrement ?? s.PriceIncrement,
            SizeIncrement = d.SizeIncrement ?? s.SizeIncrement,
            MakerFeeRate = d.MakerFeeRate ?? s.MakerFeeRate,
            TakerFeeRate = d.TakerFeeRate ?? s.TakerFeeRate,
            FeeCurrency = d.FeeCurrency ?? s.FeeCurrency,
            FeeType = d.FeeType ?? s.FeeType,
            IsSupported = d.IsSupported ?? s.IsSupported,
            StatusNote = d.StatusNote ?? s.StatusNote
        };

    private static BittradeStaticFeatureFlags? MergeFeatures(
        BittradeStaticFeatureFlags? s,
        BittradeDynamicFeatureFlags? d)
    {
        if (s is null && d is null) return null;
        s ??= new BittradeStaticFeatureFlags();
        if (d is null) return s;

        return new BittradeStaticFeatureFlags
        {
            SupportsWebSocket = d.SupportsWebSocket ?? s.SupportsWebSocket,
            SupportsMargin = d.SupportsMargin ?? s.SupportsMargin,
            SupportsStopOrder = d.SupportsStopOrder ?? s.SupportsStopOrder,
            SupportsParentOrder = d.SupportsParentOrder ?? s.SupportsParentOrder,
            SupportsCandlestick = d.SupportsCandlestick ?? s.SupportsCandlestick,
            SupportsOrderBookDelta = d.SupportsOrderBookDelta ?? s.SupportsOrderBookDelta,
            SupportsRealtimeExecutions = d.SupportsRealtimeExecutions ?? s.SupportsRealtimeExecutions,
            SupportsWithdraw = d.SupportsWithdraw ?? s.SupportsWithdraw
        };
    }

    private static BittradeStaticRateLimits? MergeRateLimits(
        BittradeStaticRateLimits? s,
        BittradeDynamicRateLimits? d)
    {
        if (s is null && d is null) return null;
        s ??= new BittradeStaticRateLimits();
        if (d is null) return s;

        return new BittradeStaticRateLimits
        {
            RequestsPerMinute = d.RequestsPerMinute ?? s.RequestsPerMinute,
            OrdersPerMinute = d.OrdersPerMinute ?? s.OrdersPerMinute
        };
    }

    private static BittradeStaticMaintenance? MergeMaintenance(
        BittradeStaticMaintenance? s,
        BittradeDynamicMaintenance? d)
    {
        if (s is null && d is null) return null;
        s ??= new BittradeStaticMaintenance();
        if (d is null) return s;

        return new BittradeStaticMaintenance
        {
            Status = MapStatus(d.Status) ?? s.Status,
            PlannedUntil = d.PlannedUntil ?? s.PlannedUntil,
            Message = d.Message ?? s.Message
        };
    }

    private static BittradeStaticMaintenanceStatus? MapStatus(BittradeDynamicMaintenanceStatus? status) =>
        status switch
        {
            null => null,
            BittradeDynamicMaintenanceStatus.Normal => BittradeStaticMaintenanceStatus.Normal,
            BittradeDynamicMaintenanceStatus.Planned => BittradeStaticMaintenanceStatus.Planned,
            BittradeDynamicMaintenanceStatus.Unplanned => BittradeStaticMaintenanceStatus.Unplanned,
            _ => null
        };
}
