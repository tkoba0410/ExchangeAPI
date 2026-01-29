using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade 用のファサード。各 API 実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BittradeExchangeClient : IPublicApi, IPrivateApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly BittradeTradingApi _tradingApi;
    private readonly BittradeAccountApi _accountApi;
    private readonly BittradeSpotHistoryApi _historyApi;
    private readonly BittradeExchangeInfoApi _exchangeInfoApi;
    private readonly IRestClient? _restClient;
    internal BittradeApiBundle? ApiBundle { get; }

    // IExchangeClient (nullable capability) に合わせる。実体は常に non-null。
    public IPublicApi? Public => this;
    public IPrivateApi? Private => this;

    public BittradeExchangeClient(
        MarketApi marketApi,
        BittradeTradingApi tradingApi,
        BittradeAccountApi accountApi,
        BittradeSpotHistoryApi historyApi,
        BittradeExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _historyApi = historyApi ?? throw new ArgumentNullException(nameof(historyApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
    }

    internal BittradeExchangeClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        if (string.IsNullOrWhiteSpace(bundle.AccountId))
        {
            throw new InvalidOperationException("BittradeApiBundle.AccountId is required to create BittradeExchangeClient.");
        }

        _marketApi = new MarketApi(bundle.Public, bundle.Markets);
        _tradingApi = new BittradeTradingApi(bundle.Private);
        _accountApi = new BittradeAccountApi(bundle.Private);
        _historyApi = new BittradeSpotHistoryApi(bundle.Private);
        _exchangeInfoApi = new BittradeExchangeInfoApi(bundle.Public);
        _restClient = bundle.RestClient;
        ApiBundle = bundle;
    }

    public BittradeExchangeClient(
        MarketApi marketApi,
        BittradeTradingApi tradingApi,
        BittradeAccountApi accountApi,
        BittradeSpotHistoryApi historyApi,
        BittradeExchangeInfoApi exchangeInfoApi,
        IRestClient restClient)
        : this(marketApi, tradingApi, accountApi, historyApi, exchangeInfoApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public BittradeExchangeClient(
        MarketApi marketApi,
        BittradeTradingApi tradingApi,
        BittradeAccountApi accountApi,
        BittradeSpotHistoryApi historyApi,
        BittradeExchangeInfoApi exchangeInfoApi,
        IRestClient restClient,
        string accountId)
        : this(marketApi, tradingApi, accountApi, historyApi, exchangeInfoApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
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

    public Task<Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>> GetHistoryKlineCallAsync(
        CommonSymbol symbol,
        string period,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetHistoryKlineCallAsync(symbol, period, size, cancellationToken);

    public Task<Call<GetTickersRequest, IReadOnlyList<Ticker>>> GetTickersCallAsync(
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickersCallAsync(cancellationToken);

    public Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>> GetHistoryTradeCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetHistoryTradeCallAsync(symbol, cancellationToken);

    // ExchangeInfo
    public Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoCallAsync(cancellationToken);

    public Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetCurrencysCallAsync(cancellationToken);

    public Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetTimestampCallAsync(cancellationToken);

    public Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesCallAsync(cancellationToken);

    public Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceLimitOrderCallAsync(symbol, side, size, price, cancellationToken);

    public Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceMarketOrderCallAsync(symbol, side, size, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetOpenOrdersCallAsync(symbol, cancellationToken);

    // SpotHistory
    public Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default) =>
        _historyApi.GetOrdersCallAsync(request, cancellationToken);

    public Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default) =>
        _historyApi.GetExecutionsCallAsync(request, cancellationToken);

    // Raw access removed from public facade.
}
