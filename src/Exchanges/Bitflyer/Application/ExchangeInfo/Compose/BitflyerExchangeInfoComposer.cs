using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Compose;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Dynamic;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Static;

namespace ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Compose;

internal static class BitflyerExchangeInfoComposer
{
    public static BitflyerStaticExchangeInfo Compose(
        BitflyerStaticExchangeInfo @static,
        BitflyerDynamicExchangeInfo? dynamic)
    {
        if (@static is null) throw new ArgumentNullException(nameof(@static));
        if (dynamic is null) return @static;

        var markets = MergeMarkets(@static.Markets, dynamic.Markets);
        var features = MergeFeatures(@static.Features, dynamic.Features);
        var rateLimits = MergeRateLimits(@static.RateLimits, dynamic.RateLimits);
        var maintenance = MergeMaintenance(@static.Maintenance, dynamic.Maintenance);

        return new BitflyerStaticExchangeInfo
        {
            Markets = markets,
            Features = features,
            RateLimits = rateLimits,
            Maintenance = maintenance,
            FeeSchedule = @static.FeeSchedule
        };
    }

    private static IReadOnlyList<BitflyerStaticMarketInfo> MergeMarkets(
        IReadOnlyList<BitflyerStaticMarketInfo> staticMarkets,
        IReadOnlyList<BitflyerDynamicMarketInfo>? dynamicMarkets)
    {
        var dynamic = (dynamicMarkets ?? Array.Empty<BitflyerDynamicMarketInfo>())
            .Where(m => !string.IsNullOrWhiteSpace(m.ProductCode))
            .ToList();

        return ExchangeInfoMerge.MergeByKey(
            staticMarkets,
            dynamic,
            staticItem => staticItem.ProductCode,
            dynamicItem => dynamicItem.ProductCode,
            MergeMarket,
            dynamicItem => new BitflyerStaticMarketInfo
            {
                Symbol = dynamicItem.Symbol ?? dynamicItem.ProductCode,
                ProductCode = dynamicItem.ProductCode,
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

    private static BitflyerStaticMarketInfo MergeMarket(BitflyerStaticMarketInfo s, BitflyerDynamicMarketInfo d) =>
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

    private static BitflyerStaticFeatureFlags? MergeFeatures(
        BitflyerStaticFeatureFlags? s,
        BitflyerDynamicFeatureFlags? d)
    {
        if (s is null && d is null) return null;
        s ??= new BitflyerStaticFeatureFlags();
        if (d is null) return s;

        return new BitflyerStaticFeatureFlags
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

    private static BitflyerStaticRateLimits? MergeRateLimits(
        BitflyerStaticRateLimits? s,
        BitflyerDynamicRateLimits? d)
    {
        if (s is null && d is null) return null;
        s ??= new BitflyerStaticRateLimits();
        if (d is null) return s;

        return new BitflyerStaticRateLimits
        {
            RequestsPerMinute = d.RequestsPerMinute ?? s.RequestsPerMinute,
            OrdersPerMinute = d.OrdersPerMinute ?? s.OrdersPerMinute
        };
    }

    private static BitflyerStaticMaintenance? MergeMaintenance(
        BitflyerStaticMaintenance? s,
        BitflyerDynamicMaintenance? d)
    {
        if (s is null && d is null) return null;
        s ??= new BitflyerStaticMaintenance();
        if (d is null) return s;

        return new BitflyerStaticMaintenance
        {
            Status = MapStatus(d.Status) ?? s.Status,
            PlannedUntil = d.PlannedUntil ?? s.PlannedUntil,
            Message = d.Message ?? s.Message
        };
    }

    private static BitflyerStaticMaintenanceStatus? MapStatus(BitflyerDynamicMaintenanceStatus? status) =>
        status switch
        {
            null => null,
            BitflyerDynamicMaintenanceStatus.Normal => BitflyerStaticMaintenanceStatus.Normal,
            BitflyerDynamicMaintenanceStatus.Planned => BitflyerStaticMaintenanceStatus.Planned,
            BitflyerDynamicMaintenanceStatus.Unplanned => BitflyerStaticMaintenanceStatus.Unplanned,
            _ => null
        };
}
