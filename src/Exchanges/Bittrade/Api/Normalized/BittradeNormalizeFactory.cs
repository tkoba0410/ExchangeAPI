using System;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized;

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
