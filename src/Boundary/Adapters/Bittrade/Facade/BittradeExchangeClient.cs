using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Boundary.Adapters.Common.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.Account;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Spec.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Facade;

/// <summary>
/// Bittrade 用のファサード。各 API 実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BittradeExchangeClient : IMarketDataApi, ITradingApi, IAccountApi, IMarginAccountApi, IExchangeInfoApi, IExchangeClient, IHasRawAccess
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    private readonly IRestClient? _restClient;
    private readonly object? _rawBundle;
    internal BittradeApiBundle? ApiBundle { get; }

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bittrade;
    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public IExchangeInfoApi Info => _exchangeInfoApi;

    public BittradeExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        IExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
    }

    internal BittradeExchangeClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        if (string.IsNullOrWhiteSpace(bundle.AccountId))
        {
            throw new InvalidOperationException("BittradeApiBundle.AccountId is required to create BittradeExchangeClient.");
        }

        _marketApi = new BittradeMarketDataApi(bundle.NormalizedMarketData, bundle.Markets);
        _tradingApi = new BittradeTradingApi(bundle.Trading);
        _accountApi = bundle.NormalizedAccount is null
            ? new NotSupportedAccountApi(ExchangeCode.Bittrade)
            : new BittradeAccountApi(bundle.NormalizedAccount);
        _exchangeInfoApi = bundle.ExchangeInfo;
        _restClient = bundle.RestClient;
        _rawBundle = bundle.RawBundle;
        ApiBundle = bundle;
    }

    public BittradeExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        IExchangeInfoApi exchangeInfoApi,
        IRestClient restClient)
        : this(marketApi, tradingApi, accountApi, exchangeInfoApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(_restClient);
        _rawBundle = normalizeBundle.RawBundle;
    }

    public BittradeExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        IExchangeInfoApi exchangeInfoApi,
        IRestClient restClient,
        string accountId)
        : this(marketApi, tradingApi, accountApi, exchangeInfoApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(_restClient, accountId);
        _rawBundle = normalizeBundle.RawBundle;
    }

    public Task<Ticker> GetTickerAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(CommonSymbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

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

    public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesAsync(cancellationToken);

    public Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesCallAsync(cancellationToken);

    public Task<OrderResult> PlaceLimitOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceLimitOrderAsync(symbol, side, size, price, cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceMarketOrderAsync(symbol, side, size, cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceStopOrderAsync(symbol, side, size, triggerPrice, cancellationToken);

    public Task<CancelResult> CancelOrderAsync(CommonSymbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderAsync(symbol, orderKey, cancellationToken);

    public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrdersAsync(symbol, cancellationToken);

    public Task<OrderStatus> GetOrderAsync(CommonSymbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrderAsync(symbol, orderKey, cancellationToken);

    public Task<IReadOnlyList<ParentOrder>> GetParentOrdersAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetParentOrdersAsync(symbol, parentOrderId, parentOrderAcceptanceId, cancellationToken);

    public Task<ParentOrderDetail> GetParentOrderAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetParentOrderAsync(symbol, parentOrderId, parentOrderAcceptanceId, cancellationToken);

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

    public Task<Call<PlaceStopOrderRequest, OrderResult>> PlaceStopOrderCallAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceStopOrderCallAsync(symbol, side, size, triggerPrice, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<GetOrdersRequest, IReadOnlyList<OpenOrder>>> GetOrdersCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrder>>> GetParentOrdersCallAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetParentOrdersCallAsync(symbol, parentOrderId, parentOrderAcceptanceId, cancellationToken);

    public Task<Call<GetParentOrderRequest, ParentOrderDetail>> GetParentOrderCallAsync(
        CommonSymbol symbol,
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetParentOrderCallAsync(symbol, parentOrderId, parentOrderAcceptanceId, cancellationToken);

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _accountApi.GetAccountExecutionsAsync(symbol, cancellationToken);

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>> GetAccountExecutionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _accountApi.GetAccountExecutionsCallAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Position>> GetOpenPositionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "MarginPositions");

    public Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "MarginCollateral");

    public Task<Call<GetOpenPositionsRequest, IReadOnlyList<Position>>> GetOpenPositionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetOpenPositionsRequest, IReadOnlyList<Position>>(new GetOpenPositionsRequest(symbol)));

    public Task<Call<GetCollateralRequest, Collateral>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall<GetCollateralRequest, Collateral>(new GetCollateralRequest()));

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    public Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoCallAsync(cancellationToken);

    public bool TryGetRaw<T>(out T raw) where T : class
    {
        raw = _rawBundle as T ?? null!;
        return raw is not null;
    }

    private static Call<TReq, TOk> NotSupportedCall<TReq, TOk>(TReq request)
    {
        var now = DateTimeOffset.UtcNow;
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: "NotSupported",
            Tags: null,
            Children: null);
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Semantic, "Feature not supported.")),
            Meta: meta);
    }
}
