using System.Collections.Generic;

namespace ExchangeApi.Contracts.Dtos;

public sealed record ExchangeInfo(
    IReadOnlyList<ExchangeMarketInfo> Markets,
    ExchangeFeatureFlags? Features,
    ExchangeRateLimits? RateLimits);

public sealed record ExchangeMarketInfo(
    string Symbol,
    string ProductCode,
    string Type,
    decimal? MinSize = null,
    decimal? PriceIncrement = null,
    decimal? SizeIncrement = null);

public sealed record ExchangeFeatureFlags(
    bool SupportsWebSocket,
    bool SupportsMargin,
    bool SupportsStopOrder,
    bool SupportsParentOrder,
    bool SupportsWithdraw);

public sealed record ExchangeRateLimits(int? RequestsPerMinute, int? OrdersPerMinute);
