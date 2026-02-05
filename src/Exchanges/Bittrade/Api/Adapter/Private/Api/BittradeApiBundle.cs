using System;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Internal;
using ExchangeApi.Transport.Wire;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;

/// <summary>
/// Bittrade API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BittradeApiBundle
{
    public BittradeNormalizedPublicApi Public { get; }
    public BittradeNormalizedPrivateApi? Private { get; }
    public IExchangeInfoProvider ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }
    public IRestClient RestClient { get; }
    public FreeText? AccountId { get; }

    public BittradeApiBundle(
        BittradeNormalizedPublicApi publicApi,
        BittradeNormalizedPrivateApi? privateApi,
        IExchangeInfoProvider exchangeInfo,
        IExchangeMarketResolver markets,
        IRestClient restClient,
        FreeText? accountId = null)
    {
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        Markets = markets ?? throw new ArgumentNullException(nameof(markets));
        RestClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        AccountId = accountId is null || accountId.Value.IsEmpty ? null : accountId;
    }

    public static BittradeApiBundle FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var hasAccountId = !string.IsNullOrWhiteSpace(accountId);

        var wireTransport = new WireTransport(restClient);
        var wire = new BittradeWireCallExecutor(wireTransport);
        var raw = new BittradeRawApi(wire);

        if (!hasAccountId)
        {
            var publicApi = new BittradeNormalizedPublicApi(raw);
            var exchangeInfo = new BittradeExchangeInfoApi(publicApi);
            var markets = new ExchangeInfoMarketResolver(exchangeInfo);
            return new BittradeApiBundle(
                publicApi: publicApi,
                privateApi: null,
                exchangeInfo: exchangeInfo,
                markets: markets,
                restClient: restClient,
                accountId: null);
        }

        var normalizedAccountId = FreeText.ParseOrThrow(accountId);
        var components = BittradeNormalizedComponentFactory.FromRaw(
            raw,
            exchangeInfo =>
            {
                var exchangeInfoApi = new BittradeExchangeInfoApi(exchangeInfo);
                var markets = new ExchangeInfoMarketResolver(exchangeInfoApi);
                return new BittradeNormalizedMarketResolver(markets);
            },
            normalizedAccountId);

        var exchangeInfoFull = new BittradeExchangeInfoApi(components.Public);
        var marketsFull = new ExchangeInfoMarketResolver(exchangeInfoFull);
        var privateApi = components.Private;
        return new BittradeApiBundle(
            publicApi: components.Public,
            privateApi: privateApi,
            exchangeInfo: exchangeInfoFull,
            markets: marketsFull,
            restClient: restClient,
            accountId: normalizedAccountId);
    }
}
