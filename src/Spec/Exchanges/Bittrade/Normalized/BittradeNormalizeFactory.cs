using System;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
using ExchangeApi.Spec.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize;

internal static class BittradeNormalizeFactory
{
    public static BittradeNormalizeBundle FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var wire = new WireTransport(restClient);
        var raw = new BittradeRawApi(wire);

        var marketData = new BittradeNormalizedMarketDataApi(raw.MarketData);
        var exchangeInfo = new BittradeNormalizedExchangeInfoApi(raw);
        var account = normalizedAccountId is null
            ? null
            : new BittradeNormalizedAccountApi(raw, normalizedAccountId);

        return new BittradeNormalizeBundle(
            marketData: marketData,
            exchangeInfo: exchangeInfo,
            account: account,
            rawBundle: raw,
            accountId: normalizedAccountId);
    }

    public static IBittradeNormalizedTradingApi CreateTradingApi(
        IRestClient restClient,
        IExchangeMarketResolver markets,
        string accountId)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var wire = new WireTransport(restClient);
        var raw = new BittradeRawApi(wire);
        return new BittradeNormalizedTradingApi(raw.Trading, markets, accountId);
    }
}
