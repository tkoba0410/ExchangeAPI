using System;
using System.Collections.Generic;
using Common.Dtos;
using Common.Enums;
namespace Exchange.Bitflyer.Abstract.Adapters;

internal static class BitflyerExchangeInfoMapper
{
    public static ExchangeInfo MapExchangeInfo(
        IReadOnlyList<ExchangeMarketInfo> markets,
        ExchangeFeatureFlags features,
        ExchangeRateLimits? rateLimits = null,
        ExchangeMaintenance? maintenance = null)
    {
        if (markets is null) throw new ArgumentNullException(nameof(markets));
        if (features is null) throw new ArgumentNullException(nameof(features));

        return new ExchangeInfo(markets, features, rateLimits, maintenance);
    }
}
