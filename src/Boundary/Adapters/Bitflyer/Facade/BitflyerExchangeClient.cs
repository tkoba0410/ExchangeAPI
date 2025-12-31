using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Account;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Margin;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Market;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using CommonTicker = ExchangeApi.Contracts.Dtos.Ticker;
using ContractSide = ExchangeApi.Common.Enums.Side;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BitflyerExchangeClient : IMarketDataApi, ITradingApi, IAccountApi, IMarginAccountApi, IExchangeInfoApi, IExchangeClient, IHasRawAccess
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IMarginAccountApi _marginApi;
    private readonly IAccountApi _accountApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    private readonly MarketApi? _marketApiConcrete;
    private readonly BitflyerAccountApi? _accountApiConcrete;
    internal BitflyerApiBundle? ApiBundle { get; }
    private readonly object? _rawBundle;

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bitflyer;
    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public IExchangeInfoApi Info => _exchangeInfoApi;

    internal BitflyerExchangeClient(
        IBitflyerRawMarketDataApi marketData,
        IBitflyerRawAccountApi account,
        IBitflyerRawPrivateTradingApi trading,
        ExchangeCode exchangeCode = ExchangeCode.Bitflyer,
        string accountId = "default",
        object? rawBundle = null)
        : this(
            marketApi: CreateMarketApi(marketData),
            tradingApi: CreateTradingApi(trading, account),
            marginApi: CreateMarginApi(account),
            accountApi: CreateAccountApi(account),
            exchangeInfoApi: new BitflyerExchangeInfoApi(),
            rawBundle: rawBundle)
    {
        ApiBundle = new BitflyerApiBundle(marketData, account, trading, rawBundle);
    }

    public BitflyerExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IMarginAccountApi marginApi,
        IAccountApi accountApi,
        IExchangeInfoApi exchangeInfoApi,
        object? rawBundle = null)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _marginApi = marginApi ?? throw new ArgumentNullException(nameof(marginApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
        _marketApiConcrete = _marketApi as MarketApi;
        _accountApiConcrete = _accountApi as BitflyerAccountApi;
        _rawBundle = rawBundle;
    }

    internal BitflyerExchangeClient(BitflyerApiBundle bundle)
        : this(
            marketApi: CreateMarketApi(bundle.MarketData),
            tradingApi: CreateTradingApi(bundle.Trading, bundle.Account),
            marginApi: CreateMarginApi(bundle.Account),
            accountApi: CreateAccountApi(bundle.Account),
            exchangeInfoApi: new BitflyerExchangeInfoApi(),
            rawBundle: bundle.RawBundle)
    {
        ApiBundle = bundle;
    }

    private static MarketApi CreateMarketApi(IBitflyerRawMarketDataApi marketData)
    {
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        return new MarketApi(marketData, markets, ExchangeCode.Bitflyer);
    }

    private static BitflyerTradingApi CreateTradingApi(
        IBitflyerRawPrivateTradingApi trading,
        IBitflyerRawAccountApi account)
    {
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        return new BitflyerTradingApi(trading, account, markets, ExchangeCode.Bitflyer);
    }

    private static BitflyerMarginApi CreateMarginApi(IBitflyerRawAccountApi account)
    {
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        return new BitflyerMarginApi(account, markets, ExchangeCode.Bitflyer);
    }

    private static BitflyerAccountApi CreateAccountApi(IBitflyerRawAccountApi account)
    {
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        return new BitflyerAccountApi(account, markets, ExchangeCode.Bitflyer);
    }

    // Market
    public Task<CommonTicker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        Symbol symbol,
        TimeSpan timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<HealthResponse> GetHealthAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        GetMarketApi().GetHealthAsync(symbol, cancellationToken);

    public Task<BoardStateResponse> GetBoardStateAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        GetMarketApi().GetBoardStateAsync(symbol, cancellationToken);

    // Trading
    public Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceLimitOrderAsync(symbol, side, size, price, cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceMarketOrderAsync(symbol, side, size, cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceStopOrderAsync(symbol, side, size, triggerPrice, cancellationToken);

    public Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderAsync(symbol, orderKey, cancellationToken);

    public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrdersAsync(symbol, cancellationToken);

    public Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrderAsync(symbol, orderKey, cancellationToken);

    // Account/Margin
    public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<Position>> GetOpenPositionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marginApi.GetOpenPositionsAsync(symbol, cancellationToken);

    public Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _marginApi.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _accountApi.GetAccountExecutionsAsync(symbol, cancellationToken);

    public Task<System.Text.Json.JsonElement> GetTradingCommissionAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        GetAccountApi().GetTradingCommissionAsync(symbol, cancellationToken);

    // ExchangeInfo
    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    private MarketApi GetMarketApi()
    {
        if (_marketApiConcrete is null)
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, "MarketRawAccess");
        }

        return _marketApiConcrete;
    }

    private BitflyerAccountApi GetAccountApi()
    {
        if (_accountApiConcrete is null)
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, "AccountRawAccess");
        }

        return _accountApiConcrete;
    }

    public bool TryGetRaw<T>(out T raw) where T : class
    {
        raw = _rawBundle as T ?? null!;
        return raw is not null;
    }
}
