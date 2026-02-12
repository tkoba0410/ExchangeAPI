using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api.Markets;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal static class NormalizeFactory
{
    public static NormalizedApi FromRaw(
        IRawApi raw,
        IMarketResolver markets,
        AccountId accountId)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return NormalizedApi.FromRaw(raw, markets, accountId);
    }
}
