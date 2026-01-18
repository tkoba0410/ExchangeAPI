using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Facade.Requests;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Account;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.History;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Market;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Facade;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BittradePublicClient : IMarketDataApi, IExchangeClient
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly ISpotHistoryApi _historyApi;
    private readonly IRestClient? _restClient;
    internal BittradeApiBundle? ApiBundle { get; }

    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public ISpotHistoryApi History => _historyApi;

    public BittradePublicClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizeBundle.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        _marketApi = new MarketApi(normalizeBundle.MarketData, markets);
        var tradingNormalized = new BittradeNotSupportedNormalizedTradingApi();
        _tradingApi = new BittradeTradingApi(tradingNormalized);
        _accountApi = new BittradeAccountApi(normalizeBundle.Account);
        _historyApi = new BittradeSpotHistoryApi(tradingNormalized, normalizeBundle.AccountId);
        _restClient = restClient;
    }

    public BittradePublicClient(IMarketDataApi marketApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        var tradingNormalized = new BittradeNotSupportedNormalizedTradingApi();
        var accountNormalized = new BittradePreconditionMissingNormalizedAccountApi(string.Empty);
        _tradingApi = new BittradeTradingApi(tradingNormalized);
        _accountApi = new BittradeAccountApi(accountNormalized);
        _historyApi = new BittradeSpotHistoryApi(tradingNormalized, null);
    }

    internal BittradePublicClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        _marketApi = new MarketApi(bundle.NormalizedMarketData, bundle.Markets);
        var tradingNormalized = new BittradeNotSupportedNormalizedTradingApi();
        _tradingApi = new BittradeTradingApi(tradingNormalized);
        _accountApi = new BittradeAccountApi(bundle.NormalizedAccount);
        _historyApi = new BittradeSpotHistoryApi(tradingNormalized, bundle.AccountId);
        _restClient = bundle.RestClient;
        ApiBundle = bundle;
    }

    public Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookCallAsync(symbol, cancellationToken);

    public Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsCallAsync(symbol, cancellationToken);

    // Raw access removed from public facade.
}
