using System;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Transport.Wire;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class ApiBundle
{
    public NormalizedPublicApi Public { get; }
    public NormalizedPrivateApi? Private { get; }
    public IExchangeInfoProvider ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }
    public IRestClient RestClient { get; }
    public AccountId? AccountId { get; }

    public ApiBundle(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi? privateApi,
        IExchangeInfoProvider exchangeInfo,
        IExchangeMarketResolver markets,
        IRestClient restClient,
        AccountId? accountId = null)
    {
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        Markets = markets ?? throw new ArgumentNullException(nameof(markets));
        RestClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        AccountId = accountId is null || accountId.Value.IsEmpty ? null : accountId;
    }

    public static ApiBundle FromRestClient(IRestClient restClient, AccountId? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var hasAccountId = accountId is { IsEmpty: false };

        var wireTransport = new WireTransport(restClient);
        var wire = new WireCallExecutor(wireTransport);
        var raw = new RawApi(wire);

        if (!hasAccountId)
        {
            var publicApi = new NormalizedPublicApi(raw);
            var exchangeInfo = new BittradeExchangeInfoApi(publicApi);
            var markets = new ExchangeInfoMarketResolver(exchangeInfo);
            return new ApiBundle(
                publicApi: publicApi,
                privateApi: null,
                exchangeInfo: exchangeInfo,
                markets: markets,
                restClient: restClient,
                accountId: null);
        }

        var normalizedAccountId = accountId!.Value;
        var components = NormalizedComponentFactory.FromRaw(
            raw,
            exchangeInfo =>
            {
                var exchangeInfoApi = new BittradeExchangeInfoApi(exchangeInfo);
                var markets = new ExchangeInfoMarketResolver(exchangeInfoApi);
                return new NormalizedMarketResolver(markets);
            },
            FreeText.ParseOrThrow(normalizedAccountId.Value));

        var exchangeInfoFull = new BittradeExchangeInfoApi(components.Public);
        var marketsFull = new ExchangeInfoMarketResolver(exchangeInfoFull);
        var privateApi = components.Private;
        return new ApiBundle(
            publicApi: components.Public,
            privateApi: privateApi,
            exchangeInfo: exchangeInfoFull,
            markets: marketsFull,
            restClient: restClient,
            accountId: normalizedAccountId);
    }
}
