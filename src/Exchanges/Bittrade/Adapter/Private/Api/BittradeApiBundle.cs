using System;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BittradeApiBundle
{
    public BittradeNormalizedTradingApi Trading { get; }
    public BittradeNormalizedMarketDataApi NormalizedMarketData { get; }
    public BittradeNormalizedAccountApi NormalizedAccount { get; }
    public IExchangeInfoApi ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }
    public IRestClient RestClient { get; }
    public string? AccountId { get; }

    public BittradeApiBundle(
        BittradeNormalizedTradingApi trading,
        BittradeNormalizedMarketDataApi normalizedMarketData,
        BittradeNormalizedAccountApi normalizedAccount,
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

        var components = BittradeNormalizedComponentFactory.FromRestClient(
            restClient,
            exchangeInfo =>
            {
                var exchangeInfoApi = new BittradeExchangeInfoApi(exchangeInfo);
                var markets = new ExchangeInfoMarketResolver(exchangeInfoApi);
                return new BittradeNormalizedMarketResolver(markets);
            },
            normalizedAccountId);

        var exchangeInfo = new BittradeExchangeInfoApi(components.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var trading = components.Trading;
        return new BittradeApiBundle(
            trading: trading,
            normalizedMarketData: components.MarketData,
            normalizedAccount: components.Account,
            exchangeInfo: exchangeInfo,
            markets: markets,
            restClient: restClient,
            accountId: normalizedAccountId);
    }
}
