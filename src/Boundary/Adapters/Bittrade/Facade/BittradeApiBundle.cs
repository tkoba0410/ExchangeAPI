using System;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Facade;

/// <summary>
/// Bittrade API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BittradeApiBundle
{
    public IBittradeRawTradingApi Trading { get; }
    public IBittradeNormalizedMarketDataApi NormalizedMarketData { get; }
    public IBittradeNormalizedAccountApi? NormalizedAccount { get; }
    public IExchangeInfoApi ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }
    public IRestClient RestClient { get; }
    public string? AccountId { get; }
    public object? RawBundle { get; }

    public BittradeApiBundle(
        IBittradeRawTradingApi trading,
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
        return new BittradeApiBundle(
            trading: normalizeBundle.RawBundle.Trading,
            normalizedMarketData: normalizeBundle.MarketData,
            normalizedAccount: normalizeBundle.Account,
            exchangeInfo: exchangeInfo,
            markets: markets,
            restClient: restClient,
            accountId: normalizedAccountId,
            rawBundle: normalizeBundle.RawBundle);
    }
}
