using System;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.ExchangeInfo;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Facade;

/// <summary>
/// Bittrade API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BittradeApiBundle
{
    public IBittradeNormalizedTradingApi Trading { get; }
    public IBittradeNormalizedMarketDataApi NormalizedMarketData { get; }
    public IBittradeNormalizedAccountApi? NormalizedAccount { get; }
    public IExchangeInfoApi ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }
    public IRestClient RestClient { get; }
    public string? AccountId { get; }
    public object? RawBundle { get; }

    public BittradeApiBundle(
        IBittradeNormalizedTradingApi trading,
        IBittradeNormalizedMarketDataApi normalizedMarketData,
        IBittradeNormalizedAccountApi? normalizedAccount,
        IExchangeInfoApi exchangeInfo,
        IExchangeMarketResolver markets,
        IRestClient restClient,
        string? accountId = null,
        object? rawBundle = null)
    {
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        NormalizedMarketData = normalizedMarketData ?? throw new ArgumentNullException(nameof(normalizedMarketData));
        NormalizedAccount = normalizedAccount;
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        Markets = markets ?? throw new ArgumentNullException(nameof(markets));
        RestClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        AccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        RawBundle = rawBundle;
    }

    public static BittradeApiBundle FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient, normalizedAccountId);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizeBundle.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var trading = normalizedAccountId is null
            ? throw new InvalidOperationException("accountId is required to create Bittrade trading API.")
            : BittradeNormalizeFactory.CreateTradingApi(restClient, markets, normalizedAccountId);
        return new BittradeApiBundle(
            trading: trading,
            normalizedMarketData: normalizeBundle.MarketData,
            normalizedAccount: normalizeBundle.Account,
            exchangeInfo: exchangeInfo,
            markets: markets,
            restClient: restClient,
            accountId: normalizedAccountId,
            rawBundle: normalizeBundle.RawBundle);
    }
}
