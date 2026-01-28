using System;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal static class BittradeNormalizeFactory
{
    public static BittradeNormalizedApi FromRestClient(
        IRestClient restClient,
        IBittradeMarketResolver markets,
        string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return BittradeNormalizedApi.FromRestClient(restClient, markets, accountId);
    }
}
