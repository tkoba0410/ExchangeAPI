using System;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized;

internal static class BitflyerNormalizeFactory
{
    public static BitflyerNormalizedApi FromRaw(
        IBitflyerRawApi raw,
        IBitflyerMarketResolver markets)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return BitflyerNormalizedApi.FromRaw(raw, markets);
    }
}
