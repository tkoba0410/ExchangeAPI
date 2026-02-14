using System;
using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Static;

internal sealed class BittradeStaticExchangeInfo
{
    public IReadOnlyList<BittradeStaticMarketInfo> Markets { get; init; } = Array.Empty<BittradeStaticMarketInfo>();
    public BittradeStaticFeatureFlags? Features { get; init; }
    public BittradeStaticRateLimits? RateLimits { get; init; }
    public BittradeStaticMaintenance? Maintenance { get; init; }
}

internal sealed class BittradeStaticMarketInfo
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

internal sealed class BittradeStaticFeatureFlags
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

internal sealed class BittradeStaticRateLimits
{
    public int? RequestsPerMinute { get; init; }
    public int? OrdersPerMinute { get; init; }
}

internal sealed class BittradeStaticMaintenance
{
    public BittradeStaticMaintenanceStatus? Status { get; init; }
    public DateTimeOffset? PlannedUntil { get; init; }
    public string? Message { get; init; }
}

internal enum BittradeStaticMaintenanceStatus
{
    Normal,
    Planned,
    Unplanned,
}
