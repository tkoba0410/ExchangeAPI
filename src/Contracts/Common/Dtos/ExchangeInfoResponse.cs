using System.Collections.Generic;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record ExchangeInfoResponse(
    IReadOnlyList<ExchangeMarketInfo> Markets,
    ExchangeFeatureFlags? Features,
    ExchangeRateLimits? RateLimits,
    ExchangeMaintenance? Maintenance);
