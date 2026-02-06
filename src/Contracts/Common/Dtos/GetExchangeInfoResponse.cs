using System.Collections.Generic;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetExchangeInfoResponse(
    IReadOnlyList<ExchangeMarketInfo> Markets,
    ExchangeFeatureFlags? Features,
    ExchangeRateLimits? RateLimits,
    ExchangeMaintenance? Maintenance);
