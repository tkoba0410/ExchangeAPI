using System;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal static class BittradeNormalizeFactory
{
    public static BittradeNormalizeBundle FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var wire = new WireTransport(restClient);
        var raw = new BittradeRawApi(wire);

        var marketData = new BittradeNormalizedMarketDataApi(raw);
        var exchangeInfo = new BittradeNormalizedExchangeInfoApi(raw);
        IBittradeNormalizedAccountApi account = normalizedAccountId is null
            ? new BittradePreconditionMissingNormalizedAccountApi(string.Empty)
            : new BittradeNormalizedAccountApi(raw, normalizedAccountId);

        return new BittradeNormalizeBundle(
            raw: raw,
            marketData: marketData,
            exchangeInfo: exchangeInfo,
            account: account,
            accountId: normalizedAccountId);
    }

}
