using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using Common.Contract.Contracts;
using Common.Contract.Dtos;

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
    public Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<MarketExecution>> GetMarketExecutionsAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        string symbol,
        string timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    // Trading
    public Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken cancellationToken = default) =>
        _tradingApi.SendOrderAsync(request, cancellationToken);

    public Task<CancelResult> CancelOrderAsync(string productCode, string childOrderAcceptanceId, CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderAsync(productCode, childOrderAcceptanceId, cancellationToken);

    public Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(string productCode, CancellationToken cancellationToken = default) =>
        _tradingApi.GetOpenOrdersAsync(productCode, cancellationToken);

    public Task<OrderStatus> PollOrderStatusAsync(string productCode, string childOrderAcceptanceId, TimeSpan? pollInterval = null, int maxAttempts = 30, CancellationToken cancellationToken = default) =>
        _tradingApi.PollOrderStatusAsync(productCode, childOrderAcceptanceId, pollInterval, maxAttempts, cancellationToken);

    // Account/Margin
    public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<Position>> GetOpenPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _marginApi.GetOpenPositionsAsync(productCode, cancellationToken);

    public Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _marginApi.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<AccountExecution>> GetAccountExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _accountApi.GetAccountExecutionsAsync(productCode, cancellationToken);

    // ExchangeInfo
    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);
}
