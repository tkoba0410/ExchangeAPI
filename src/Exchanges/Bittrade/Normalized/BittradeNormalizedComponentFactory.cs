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
        Func<BittradeNormalizedExchangeInfoApi, IBittradeMarketResolver> marketResolverFactory,
        string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (marketResolverFactory is null) throw new ArgumentNullException(nameof(marketResolverFactory));

        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var wire = new WireTransport(restClient);
        var raw = new BittradeRawApi(wire);

        var exchangeInfo = new BittradeNormalizedExchangeInfoApi(raw);
        var markets = marketResolverFactory(exchangeInfo);
        var marketData = new BittradeNormalizedMarketDataApi(raw);
        var account = new BittradeNormalizedAccountApi(raw, normalizedAccountId);
        var trading = new BittradeNormalizedTradingApi(raw, markets, normalizedAccountId);

        return new BittradeNormalizedComponents(
            marketData: marketData,
            exchangeInfo: exchangeInfo,
            account: account,
            trading: trading,
            accountId: normalizedAccountId);
    }
}
