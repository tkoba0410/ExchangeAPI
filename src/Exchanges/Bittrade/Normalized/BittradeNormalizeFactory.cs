using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal static class BittradeNormalizeFactory
{
    public static BittradeNormalizedApi FromRaw(
        IBittradeRawApi raw,
        IBittradeMarketResolver markets,
        FreeText accountId)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return BittradeNormalizedApi.FromRaw(raw, markets, accountId);
    }
}
