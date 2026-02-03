using System;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized;

internal static class BittradeNormalizedComponentFactory
{
    public static BittradeNormalizedComponents FromRaw(
        IBittradeRawApi raw,
        Func<BittradeNormalizedPublicApi, IBittradeMarketResolver> marketResolverFactory,
        FreeText? accountId = null)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (marketResolverFactory is null) throw new ArgumentNullException(nameof(marketResolverFactory));

        var normalizedAccountId = accountId is null || accountId.Value.IsEmpty ? null : accountId;
        var publicApi = new BittradeNormalizedPublicApi(raw);
        var markets = marketResolverFactory(publicApi);
        var privateApi = new BittradeNormalizedPrivateApi(raw, markets, normalizedAccountId);

        return new BittradeNormalizedComponents(
            publicApi: publicApi,
            privateApi: privateApi,
            accountId: normalizedAccountId);
    }
}
