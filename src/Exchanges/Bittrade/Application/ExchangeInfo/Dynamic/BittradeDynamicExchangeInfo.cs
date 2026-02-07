using System;
using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Dynamic;

internal sealed class BittradeDynamicExchangeInfo
{
    public IReadOnlyList<BittradeDynamicMarketInfo> Markets { get; init; } = Array.Empty<BittradeDynamicMarketInfo>();
    public BittradeDynamicFeatureFlags? Features { get; init; }
    public BittradeDynamicRateLimits? RateLimits { get; init; }
    public BittradeDynamicMaintenance? Maintenance { get; init; }
}

internal sealed class BittradeDynamicMarketInfo
{
    public string? Symbol { get; init; }
    public string? ProductCode { get; init; }
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

internal sealed class BittradeDynamicFeatureFlags
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

internal sealed class BittradeDynamicRateLimits
{
    public int? RequestsPerMinute { get; init; }
    public int? OrdersPerMinute { get; init; }
}

internal sealed class BittradeDynamicMaintenance
{
    public BittradeDynamicMaintenanceStatus? Status { get; init; }
    public DateTimeOffset? PlannedUntil { get; init; }
    public string? Message { get; init; }
}

internal enum BittradeDynamicMaintenanceStatus
{
    Normal,
    Planned,
    Unplanned,
}
