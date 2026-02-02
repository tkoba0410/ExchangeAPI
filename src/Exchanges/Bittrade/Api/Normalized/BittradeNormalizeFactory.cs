using System;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized;

internal static class BittradeNormalizeFactory
{
    public static BittradeNormalizedApi FromRaw(
        IBittradeRawApi raw,
        IBittradeMarketResolver markets,
        string? accountId = null)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return BittradeNormalizedApi.FromRaw(raw, markets, accountId);
    }
}
