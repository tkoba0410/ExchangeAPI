using System;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Call;
using ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
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

        var marketData = new BittradeNormalizedMarketDataApi(raw.MarketData);
        var exchangeInfo = new BittradeNormalizedExchangeInfoApi(raw);
        IBittradeNormalizedAccountApi account = normalizedAccountId is null
            ? new BittradeNotSupportedNormalizedAccountApi(string.Empty)
            : new BittradeNormalizedAccountApi(raw, normalizedAccountId);

        return new BittradeNormalizeBundle(
            marketData: marketData,
            exchangeInfo: exchangeInfo,
            account: account,
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
