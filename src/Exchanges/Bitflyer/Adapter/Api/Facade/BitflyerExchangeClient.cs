using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Account;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Market;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.History;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Trading;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using CommonTicker = ExchangeApi.Contracts.Dtos.Market.Ticker;
using ContractSide = ExchangeApi.Contracts.Common.DomainCommon.Enums.Side;
using ExchangeApi.Contracts.Common.CallCommon;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BitflyerExchangeClient : IMarketDataApi, ITradingApi, IAccountApi, IExchangeClient, IHasRawAccess
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly ISpotHistoryApi _historyApi;
    private readonly MarketApi? _marketApiConcrete;
    private readonly BitflyerAccountApi? _accountApiConcrete;
    internal BitflyerApiBundle? ApiBundle { get; }
    private readonly object? _rawBundle;

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bitflyer;
    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public ISpotHistoryApi History => _historyApi;

    internal BitflyerExchangeClient(
        BitflyerNormalizedMarketDataFacade marketData,
        IBitflyerNormalizedAccountApi account,
        IBitflyerNormalizedTradingApi trading,
        ExchangeCode exchangeCode = ExchangeCode.Bitflyer,
        object? rawBundle = null)
        : this(
            marketApi: CreateMarketApi(marketData),
            tradingApi: CreateTradingApi(trading),
            accountApi: CreateAccountApi(account),
            historyApi: CreateSpotHistoryApi(trading, account),
            rawBundle: rawBundle)
    {
        ApiBundle = new BitflyerApiBundle(marketData, account, trading, rawBundle);
    }

    public BitflyerExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        ISpotHistoryApi historyApi,
        object? rawBundle = null)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _historyApi = historyApi ?? throw new ArgumentNullException(nameof(historyApi));
        _marketApiConcrete = _marketApi as MarketApi;
        _accountApiConcrete = _accountApi as BitflyerAccountApi;
        _rawBundle = rawBundle;
    }

    internal BitflyerExchangeClient(BitflyerApiBundle bundle)
        : this(
            marketApi: CreateMarketApi(bundle.MarketData),
            tradingApi: CreateTradingApi(bundle.Trading),
            accountApi: CreateAccountApi(bundle.Account),
            historyApi: CreateSpotHistoryApi(bundle.Trading, bundle.Account),
            rawBundle: bundle.RawBundle)
    {
        ApiBundle = bundle;
    }

    public static BitflyerExchangeClient FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var normalized = BitflyerNormalizedApi.FromRestClient(restClient);
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var accountApi = BitflyerNormalizeFactory.CreateAccountApi(restClient, markets);
        var tradingApi = BitflyerNormalizeFactory.CreateTradingApi(restClient, markets);
        var historyApi = CreateSpotHistoryApi(tradingApi, accountApi);

        return new BitflyerExchangeClient(
            marketApi: CreateMarketApi(normalized.MarketData),
            tradingApi: CreateTradingApi(tradingApi),
            accountApi: CreateAccountApi(accountApi),
            historyApi: historyApi,
            rawBundle: null);
    }

    private static MarketApi CreateMarketApi(BitflyerNormalizedMarketDataFacade marketData)
    {
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        return new MarketApi(marketData, markets);
    }

    private static BitflyerTradingApi CreateTradingApi(IBitflyerNormalizedTradingApi trading) =>
        new(trading);

    private static BitflyerAccountApi CreateAccountApi(IBitflyerNormalizedAccountApi account) =>
        new(account);

    private static ISpotHistoryApi CreateSpotHistoryApi(
        IBitflyerNormalizedTradingApi trading,
        IBitflyerNormalizedAccountApi account) =>
        new BitflyerSpotHistoryApi(trading, account);

    // Market
    public Task<Call<GetTickerRequest, CommonTicker>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookCallAsync(symbol, cancellationToken);

    public Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsCallAsync(symbol, cancellationToken);

    // Trading
    public Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceLimitOrderCallAsync(symbol, side, size, price, cancellationToken);

    public Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        CancellationToken cancellationToken = default) =>
        _tradingApi.PlaceMarketOrderCallAsync(symbol, side, size, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _tradingApi.GetOpenOrdersCallAsync(symbol, cancellationToken);

    // Account
    public Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesCallAsync(cancellationToken);

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
