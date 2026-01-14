using System;
using System.Collections.Generic;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;

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
