using System;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api.Markets;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized;

internal static class NormalizeFactory
{
    public static NormalizedApi FromRaw(
        IRawApi raw,
        IMarketResolver markets)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return NormalizedApi.FromRaw(raw, markets);
    }
}
