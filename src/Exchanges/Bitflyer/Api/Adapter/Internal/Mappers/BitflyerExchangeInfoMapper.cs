using System;
using System.Collections.Generic;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Primitives.DomainCommon.Enums;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;

internal static class BitflyerExchangeInfoMapper
{
    public static ExchangeInfoDto MapExchangeInfo(
        IReadOnlyList<ExchangeMarketInfo> markets,
        ExchangeFeatureFlags features,
        ExchangeRateLimits? rateLimits = null,
        ExchangeMaintenance? maintenance = null)
    {
        if (markets is null) throw new ArgumentNullException(nameof(markets));
        if (features is null) throw new ArgumentNullException(nameof(features));

        return new ExchangeInfoDto(markets, features, rateLimits, maintenance);
    }
}
