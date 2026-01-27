using System;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.NotSupported;
using ExchangeApi.Contracts.Facade.Interfaces;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BittradeApiBundle
{
    public IBittradeNormalizedTradingApi Trading { get; }
    public IBittradeNormalizedMarketDataApi NormalizedMarketData { get; }
    public IBittradeNormalizedAccountApi NormalizedAccount { get; }
    public IExchangeInfoApi ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }
    public IRestClient RestClient { get; }
    public string? AccountId { get; }

    public BittradeApiBundle(
        IBittradeNormalizedTradingApi trading,
        IBittradeNormalizedMarketDataApi normalizedMarketData,
        IBittradeNormalizedAccountApi normalizedAccount,
        IExchangeInfoApi exchangeInfo,
        IExchangeMarketResolver markets,
        IRestClient restClient,
        string? accountId = null)
    {
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        NormalizedMarketData = normalizedMarketData ?? throw new ArgumentNullException(nameof(normalizedMarketData));
        NormalizedAccount = normalizedAccount ?? throw new ArgumentNullException(nameof(normalizedAccount));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        Markets = markets ?? throw new ArgumentNullException(nameof(markets));
        RestClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        AccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }

    public static BittradeApiBundle FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient, normalizedAccountId);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizeBundle.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var normalizedMarkets = new BittradeNormalizedMarketResolver(markets);
        IBittradeNormalizedTradingApi trading = normalizedAccountId is null
            ? new BittradePreconditionMissingNormalizedTradingApi(string.Empty)
            : new BittradeNormalizedTradingApi(normalizeBundle.Raw, normalizedMarkets, normalizedAccountId);
        return new BittradeApiBundle(
            trading: trading,
            normalizedMarketData: normalizeBundle.MarketData,
            normalizedAccount: normalizeBundle.Account,
            exchangeInfo: exchangeInfo,
            markets: markets,
            restClient: restClient,
            accountId: normalizedAccountId);
    }
}
