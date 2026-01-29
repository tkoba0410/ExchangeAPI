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
    public BittradeNormalizedPublicApi Public { get; }
    public BittradeNormalizedPrivateApi Private { get; }
    public IExchangeInfoProvider ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }
    public IRestClient RestClient { get; }
    public string? AccountId { get; }

    public BittradeApiBundle(
        BittradeNormalizedPublicApi publicApi,
        BittradeNormalizedPrivateApi privateApi,
        IExchangeInfoProvider exchangeInfo,
        IExchangeMarketResolver markets,
        IRestClient restClient,
        string? accountId = null)
    {
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
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

        var exchangeInfo = new BittradeExchangeInfoApi(components.Public);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var privateApi = components.Private;
        return new BittradeApiBundle(
            publicApi: components.Public,
            privateApi: privateApi,
            exchangeInfo: exchangeInfo,
            markets: markets,
            restClient: restClient,
            accountId: normalizedAccountId);
    }
}
