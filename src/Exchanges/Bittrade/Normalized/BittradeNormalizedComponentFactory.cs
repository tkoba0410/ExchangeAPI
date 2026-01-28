using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal static class BittradeNormalizedComponentFactory
{
    public static BittradeNormalizedComponents FromRestClient(
        IRestClient restClient,
        Func<BittradeNormalizedPublicApi, IBittradeMarketResolver> marketResolverFactory,
        string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (marketResolverFactory is null) throw new ArgumentNullException(nameof(marketResolverFactory));

        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var wire = new WireTransport(restClient);
        var raw = new BittradeRawApi(wire);

        var publicApi = new BittradeNormalizedPublicApi(raw);
        var markets = marketResolverFactory(publicApi);
        var privateApi = new BittradeNormalizedPrivateApi(raw, markets, normalizedAccountId);

        return new BittradeNormalizedComponents(
            publicApi: publicApi,
            privateApi: privateApi,
            accountId: normalizedAccountId);
    }
}
