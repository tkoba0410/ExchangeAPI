using System;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized;

internal static class BitflyerNormalizeFactory
{
    public static BitflyerNormalizedApi FromRestClient(
        IRestClient restClient,
        IBitflyerMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return BitflyerNormalizedApi.FromRestClient(restClient, markets);
    }
}
