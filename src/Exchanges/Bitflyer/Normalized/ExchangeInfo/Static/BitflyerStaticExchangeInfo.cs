using System;
using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.ExchangeInfo.Static;

internal sealed class BitflyerStaticExchangeInfo
{
    public IReadOnlyList<BitflyerStaticMarketInfo> Markets { get; init; } = Array.Empty<BitflyerStaticMarketInfo>();
    public BitflyerStaticFeatureFlags? Features { get; init; }
    public BitflyerStaticRateLimits? RateLimits { get; init; }
    public BitflyerStaticMaintenance? Maintenance { get; init; }
    public BitflyerStaticFeeSchedule? FeeSchedule { get; init; }
}

internal sealed class BitflyerStaticMarketInfo
{
    public string Symbol { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string Type { get; init; } = "Spot";
    public decimal? MinSize { get; init; }
    public decimal? MaxSize { get; init; }
    public decimal? MinNotional { get; init; }
    public decimal? PriceIncrement { get; init; }
    public decimal? SizeIncrement { get; init; }
    public decimal? MakerFeeRate { get; init; }
    public decimal? TakerFeeRate { get; init; }
    public string? FeeCurrency { get; init; }
    public string? FeeType { get; init; }
    public bool? IsSupported { get; init; }
    public string? StatusNote { get; init; }
}

internal sealed class BitflyerStaticFeatureFlags
{
    public bool SupportsWebSocket { get; init; }
    public bool SupportsMargin { get; init; }
    public bool SupportsStopOrder { get; init; }
    public bool SupportsParentOrder { get; init; }
    public bool SupportsCandlestick { get; init; }
    public bool SupportsOrderBookDelta { get; init; }
    public bool SupportsRealtimeExecutions { get; init; }
    public bool SupportsWithdraw { get; init; }
}

internal sealed class BitflyerStaticRateLimits
{
    public int? RequestsPerMinute { get; init; }
    public int? OrdersPerMinute { get; init; }
}

internal sealed class BitflyerStaticMaintenance
{
    public BitflyerStaticMaintenanceStatus? Status { get; init; }
    public DateTimeOffset? PlannedUntil { get; init; }
    public string? Message { get; init; }
}

internal enum BitflyerStaticMaintenanceStatus
{
    Normal,
    Planned,
    Unplanned,
}

internal sealed class BitflyerStaticFeeSchedule
{
    public string Scope { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public IReadOnlyList<BitflyerStaticFeeTier> Tiers { get; init; } = Array.Empty<BitflyerStaticFeeTier>();
}

internal sealed class BitflyerStaticFeeTier
{
    public decimal MinJpy { get; init; }
    public decimal? MaxJpy { get; init; }
    public decimal Rate { get; init; }
}
