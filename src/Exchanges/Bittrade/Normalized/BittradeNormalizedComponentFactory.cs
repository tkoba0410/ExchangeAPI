using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal static class BittradeNormalizedComponentFactory
{
    public static BittradeNormalizedComponents FromRaw(
        IBittradeRawApi raw,
        Func<BittradeNormalizedPublicApi, IBittradeMarketResolver> marketResolverFactory,
        FreeText accountId)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (marketResolverFactory is null) throw new ArgumentNullException(nameof(marketResolverFactory));
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var normalizedAccountId = accountId;
        var publicApi = new BittradeNormalizedPublicApi(raw);
        var markets = marketResolverFactory(publicApi);
        var privateApi = new BittradeNormalizedPrivateApi(raw, markets, normalizedAccountId);

        return new BittradeNormalizedComponents(
            publicApi: publicApi,
            privateApi: privateApi,
            accountId: normalizedAccountId);
    }
}
