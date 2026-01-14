using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Contracts.Common.DomainCommon.Types.Symbol;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Shared.Adapter.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Account;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.History;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Market;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Contracts.Common.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Facade;

/// <summary>
/// Bittrade 用のファサード。各 API 実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BittradeExchangeClient : IMarketDataApi, ITradingApi, IAccountApi, IExchangeClient, IHasRawAccess
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly ISpotHistoryApi _historyApi;
    private readonly IRestClient? _restClient;
    private readonly object? _rawBundle;
    internal BittradeApiBundle? ApiBundle { get; }

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bittrade;
    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public ISpotHistoryApi History => _historyApi;

    public BittradeExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        ISpotHistoryApi historyApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _historyApi = historyApi ?? throw new ArgumentNullException(nameof(historyApi));
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
        _historyApi = new BittradeSpotHistoryApi(bundle.Trading, bundle.AccountId);
        _restClient = bundle.RestClient;
        _rawBundle = bundle.RawBundle;
        ApiBundle = bundle;
    }

    public BittradeExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        ISpotHistoryApi historyApi,
        IRestClient restClient)
        : this(marketApi, tradingApi, accountApi, historyApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(_restClient);
        _rawBundle = normalizeBundle.RawBundle;
    }

    public BittradeExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        ISpotHistoryApi historyApi,
        IRestClient restClient,
        string accountId)
        : this(marketApi, tradingApi, accountApi, historyApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(_restClient, accountId);
        _rawBundle = normalizeBundle.RawBundle;
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
