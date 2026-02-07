using System;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized;

internal static class BitflyerNormalizeFactory
{
    public static BitflyerNormalizedApi FromRaw(
        IRawApi raw,
        IBitflyerMarketResolver markets)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return BitflyerNormalizedApi.FromRaw(raw, markets);
    }
}
