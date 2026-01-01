using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Boundary.Adapters.Common.NotSupported;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Facade;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BittradePublicClient : IMarketDataApi, IExchangeInfoApi, IExchangeClient, IHasRawAccess
{
    private readonly IMarketDataApi _marketApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly IRestClient? _restClient;
    private readonly object? _rawBundle;
    internal BittradeApiBundle? ApiBundle { get; }

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bittrade;
    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public IExchangeInfoApi Info => _exchangeInfoApi;

    public BittradePublicClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizeBundle.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        _marketApi = new BittradeMarketDataApi(normalizeBundle.MarketData, markets);
        _exchangeInfoApi = exchangeInfo;
        _tradingApi = new NotSupportedTradingApi(ExchangeCode.Bittrade);
        _accountApi = new NotSupportedAccountApi(ExchangeCode.Bittrade);
        _restClient = restClient;
        _rawBundle = normalizeBundle.RawBundle;
    }

    public BittradePublicClient(IMarketDataApi marketApi, IExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
        _tradingApi = new NotSupportedTradingApi(ExchangeCode.Bittrade);
        _accountApi = new NotSupportedAccountApi(ExchangeCode.Bittrade);
    }

    internal BittradePublicClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        _marketApi = new BittradeMarketDataApi(bundle.NormalizedMarketData, bundle.Markets);
        _exchangeInfoApi = bundle.ExchangeInfo;
        _tradingApi = new NotSupportedTradingApi(ExchangeCode.Bittrade);
        _accountApi = new NotSupportedAccountApi(ExchangeCode.Bittrade);
        _restClient = bundle.RestClient;
        _rawBundle = bundle.RawBundle;
        ApiBundle = bundle;
    }

    public Task<Ticker> GetTickerAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(CommonSymbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    public Task<ApiCall<GetTickerRequest, Ticker, ApiError>> GetTickerCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<ApiCall<GetOrderBookRequest, OrderBook, ApiError>> GetOrderBookCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookCallAsync(symbol, cancellationToken);

    public Task<ApiCall<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>, ApiError>> GetMarketExecutionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsCallAsync(symbol, cancellationToken);

    public Task<ApiCall<GetExchangeInfoRequest, ExchangeInfo, ApiError>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoCallAsync(cancellationToken);

    public bool TryGetRaw<T>(out T raw) where T : class
    {
        raw = _rawBundle as T ?? null!;
        return raw is not null;
    }
}
