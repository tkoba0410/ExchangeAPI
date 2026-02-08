using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal static class NormalizedComponentFactory
{
    public static NormalizedComponents FromRaw(
        IBittradeRawApi raw,
        Func<NormalizedPublicApi, IBittradeMarketResolver> marketResolverFactory,
        AccountId accountId)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (marketResolverFactory is null) throw new ArgumentNullException(nameof(marketResolverFactory));
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var normalizedAccountId = accountId;
        var publicApi = new NormalizedPublicApi(raw);
        var markets = marketResolverFactory(publicApi);
        var privateApi = new NormalizedPrivateApi(raw, markets, normalizedAccountId);

        return new NormalizedComponents(
            publicApi: publicApi,
            privateApi: privateApi,
            accountId: normalizedAccountId);
    }
}
