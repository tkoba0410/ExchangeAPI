using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Facade.Requests;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BittradePublicClient : IMarketDataApi, IExchangeClient
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi? _tradingApi;
    private readonly IAccountApi? _accountApi;
    private readonly ISpotHistoryApi? _historyApi;
    private readonly IRestClient? _restClient;
    internal BittradeApiBundle? ApiBundle { get; }

    public IMarketDataApi? Market => _marketApi;
    public ITradingApi? Trading => _tradingApi;
    public IAccountApi? Account => _accountApi;
    public ISpotHistoryApi? History => _historyApi;

    public BittradePublicClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        // 公開APIのみ: market/exchangeInfo 取得に限定し、Trading/Account/History は提供しない。
        var bundle = BittradeApiBundle.FromRestClient(restClient, accountId: null);
        _marketApi = new MarketApi(bundle.NormalizedMarketData, bundle.Markets);
        _tradingApi = null;
        _accountApi = null;
        _historyApi = null;
        _restClient = restClient;
    }

    public BittradePublicClient(IMarketDataApi marketApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = null;
        _accountApi = null;
        _historyApi = null;
    }

    internal BittradePublicClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        _marketApi = new MarketApi(bundle.NormalizedMarketData, bundle.Markets);
        _tradingApi = null;
        _accountApi = null;
        _historyApi = null;
        _restClient = bundle.RestClient;
        ApiBundle = bundle;
    }

    public Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDetailMergedCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        GetDepthCallAsync(symbol, cancellationToken);

    public Task<Call<GetTickerRequest, Ticker>> GetDetailMergedCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetDepthCallAsync(
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

    // Raw access removed from public facade.
}
