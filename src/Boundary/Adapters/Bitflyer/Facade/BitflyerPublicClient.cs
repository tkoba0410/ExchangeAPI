using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Boundary.Adapters.Common.NotSupported;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Market;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Facade;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using CommonTicker = ExchangeApi.Contracts.Dtos.Ticker;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BitflyerPublicClient : IMarketDataApi, IExchangeInfoApi, IExchangeClient, IHasRawAccess
{
    private readonly MarketApi _marketApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly object? _rawBundle;

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bitflyer;
    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public IExchangeInfoApi Info => _exchangeInfoApi;

    internal BitflyerPublicClient(BitflyerNormalizedMarketDataFacade marketData, object? rawBundle = null)
    {
        if (marketData is null) throw new ArgumentNullException(nameof(marketData));
        _exchangeInfoApi = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(_exchangeInfoApi);
        _marketApi = new MarketApi(marketData, markets, ExchangeCode.Bitflyer);
        _tradingApi = new NotSupportedTradingApi(ExchangeCode.Bitflyer);
        _accountApi = new NotSupportedAccountApi(ExchangeCode.Bitflyer);
        _rawBundle = rawBundle;
    }

    public Task<CommonTicker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(Symbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<BitflyerHealthNormalized> GetHealthAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetHealthAsync(symbol, cancellationToken);

    public Task<BitflyerBoardStateNormalized> GetBoardStateAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardStateAsync(symbol, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    public Task<ApiCall<GetTickerRequest, CommonTicker, ApiError>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<ApiCall<GetOrderBookRequest, OrderBook, ApiError>> GetOrderBookCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookCallAsync(symbol, cancellationToken);

    public Task<ApiCall<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>, ApiError>> GetMarketExecutionsCallAsync(
        Symbol symbol,
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
