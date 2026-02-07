using System;
using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Dynamic;

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
    public decimal? MinSize { get; set; }
    public decimal? MaxSize { get; set; }
    public decimal? MinNotional { get; set; }
    public decimal? PriceIncrement { get; set; }
    public decimal? SizeIncrement { get; set; }
    public decimal? MakerFeeRate { get; set; }
    public decimal? TakerFeeRate { get; set; }
    public string? FeeCurrency { get; set; }
    public string? FeeType { get; set; }
    public bool? IsSupported { get; set; }
    public string? StatusNote { get; set; }
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
