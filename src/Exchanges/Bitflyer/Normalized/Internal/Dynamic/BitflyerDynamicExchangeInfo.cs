using System;
using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Dynamic;

internal sealed class BitflyerDynamicExchangeInfo
{
    public IReadOnlyList<BitflyerDynamicMarketInfo>? Markets { get; init; }
    public BitflyerDynamicFeatureFlags? Features { get; init; }
    public BitflyerDynamicRateLimits? RateLimits { get; init; }
    public BitflyerDynamicMaintenance? Maintenance { get; init; }
}

internal sealed class BitflyerDynamicMarketInfo
{
    public string ProductCode { get; init; } = string.Empty;
    public string? Symbol { get; init; }
    public string? Type { get; init; }
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

internal sealed class BitflyerDynamicFeatureFlags
{
    public bool? SupportsWebSocket { get; init; }
    public bool? SupportsMargin { get; init; }
    public bool? SupportsStopOrder { get; init; }
    public bool? SupportsParentOrder { get; init; }
    public bool? SupportsCandlestick { get; init; }
    public bool? SupportsOrderBookDelta { get; init; }
    public bool? SupportsRealtimeExecutions { get; init; }
    public bool? SupportsWithdraw { get; init; }
}

internal sealed class BitflyerDynamicRateLimits
{
    public int? RequestsPerMinute { get; init; }
    public int? OrdersPerMinute { get; init; }
}

internal sealed class BitflyerDynamicMaintenance
{
    public BitflyerDynamicMaintenanceStatus? Status { get; init; }
    public DateTimeOffset? PlannedUntil { get; init; }
    public string? Message { get; init; }
}

internal enum BitflyerDynamicMaintenanceStatus
{
    Normal,
    Planned,
    Unplanned,
}
