using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using Common.Contract.Interfaces;
using Common.Contract.Enums;
using Common.Contract.Dtos;
using ContractSide = Common.Contract.Enums.Side;

namespace Exchange.Bitflyer.Abstract;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BitflyerExchangeClient : IMarketDataApi, ITradingApi, IMarginAccountApi
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IMarginAccountApi _marginApi;
    private readonly IAccountApi _accountApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    internal BitflyerApiBundle? ApiBundle { get; }
    public BitflyerRawApiClient? Raw { get; }

    public BitflyerExchangeClient(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi,
        IBitflyerPrivateTradingApi privateTradingApi,
        string exchangeId = "bitFlyer",
        string accountId = "default")
        : this(
            marketApi: new BitflyerMarketApi(publicApi, exchangeId),
            tradingApi: new BitflyerTradingApi(privateTradingApi, privateApi, exchangeId),
            marginApi: new BitflyerMarginApi(privateApi, exchangeId),
            accountApi: new BitflyerAccountApi(privateApi, exchangeId),
            exchangeInfoApi: new BitflyerExchangeInfoApi())
    {
        ApiBundle = new BitflyerApiBundle(publicApi, privateApi, privateTradingApi);
        Raw = new BitflyerRawApiClient(publicApi, privateApi, privateTradingApi);
    }

    public BitflyerExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IMarginAccountApi marginApi,
        IAccountApi accountApi,
        IExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _marginApi = marginApi ?? throw new ArgumentNullException(nameof(marginApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
    }

    internal BitflyerExchangeClient(BitflyerApiBundle bundle)
        : this(
            marketApi: new BitflyerMarketApi(bundle.PublicApi, "bitFlyer"),
            tradingApi: new BitflyerTradingApi(bundle.PrivateTradingApi, bundle.PrivateApi, "bitFlyer"),
            marginApi: new BitflyerMarginApi(bundle.PrivateApi, "bitFlyer"),
            accountApi: new BitflyerAccountApi(bundle.PrivateApi, "bitFlyer"),
            exchangeInfoApi: new BitflyerExchangeInfoApi())
    {
        ApiBundle = bundle;
    }

    // Market
    public Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
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

    // Trading
    public Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        ContractSide side,
        decimal size,
        decimal price,
        string? clientOrderId = null,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceLimitOrderAsync(symbol, side, size, price, clientOrderId, cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        ContractSide side,
        decimal size,
        string? clientOrderId = null,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceMarketOrderAsync(symbol, side, size, clientOrderId, cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        ContractSide side,
        decimal size,
        decimal triggerPrice,
        string? clientOrderId = null,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceStopOrderAsync(symbol, side, size, triggerPrice, clientOrderId, cancellationToken);

    public Task<CancelResult> CancelOrderAsync(Symbol symbol, string childOrderAcceptanceId, CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderAsync(symbol, childOrderAcceptanceId, cancellationToken);

    public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrdersAsync(symbol, cancellationToken);

    public Task<OrderStatus> PollOrderStatusAsync(Symbol symbol, string orderId, TimeSpan? pollInterval = null, int maxAttempts = 30, CancellationToken cancellationToken = default) =>
        _tradingApi.PollOrderStatusAsync(symbol, orderId, pollInterval, maxAttempts, cancellationToken);

    // Account/Margin
    public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<Position>> GetOpenPositionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marginApi.GetOpenPositionsAsync(symbol, cancellationToken);

    public Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _marginApi.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _accountApi.GetAccountExecutionsAsync(symbol, cancellationToken);

    // ExchangeInfo
    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);
}
