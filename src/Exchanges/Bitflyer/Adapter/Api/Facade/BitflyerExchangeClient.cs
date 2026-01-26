using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Account;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Market;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.History;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Trading;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.Market.Ticker;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BitflyerExchangeClient : IMarketDataApi, ITradingApi, IAccountApi, IExchangeClient
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly ISpotHistoryApi _historyApi;
    internal BitflyerApiBundle? ApiBundle { get; }
    // IExchangeClient (nullable capability) に合わせる。実体は常に non-null。
    public IMarketDataApi? Market => _marketApi;
    public ITradingApi? Trading => _tradingApi;
    public IAccountApi? Account => _accountApi;
    public ISpotHistoryApi? History => _historyApi;

    internal BitflyerExchangeClient(
        IBitflyerNormalizedApi normalized,
        ExchangeCode exchangeCode = ExchangeCode.Bitflyer,
        object? rawBundle = null)
        : this(
            marketApi: CreateMarketApi(normalized),
            tradingApi: CreateTradingApi(normalized),
            accountApi: CreateAccountApi(normalized),
            historyApi: CreateSpotHistoryApi(normalized))
    {
        ApiBundle = new BitflyerApiBundle(normalized);
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
    }

    internal BitflyerExchangeClient(BitflyerApiBundle bundle)
        : this(
            marketApi: CreateMarketApi(bundle.Normalized),
            tradingApi: CreateTradingApi(bundle.Normalized),
            accountApi: CreateAccountApi(bundle.Normalized),
            historyApi: CreateSpotHistoryApi(bundle.Normalized))
    {
        ApiBundle = bundle;
    }

    public static BitflyerExchangeClient FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizeFactory.FromRestClient(restClient, markets);

        return new BitflyerExchangeClient(
            marketApi: CreateMarketApi(normalized),
            tradingApi: CreateTradingApi(normalized),
            accountApi: CreateAccountApi(normalized),
            historyApi: CreateSpotHistoryApi(normalized));
    }

    private static MarketApi CreateMarketApi(IBitflyerNormalizedApi normalized)
    {
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        return new MarketApi(normalized, markets);
    }

    private static BitflyerTradingApi CreateTradingApi(IBitflyerNormalizedApi normalized) =>
        new(normalized);

    private static BitflyerAccountApi CreateAccountApi(IBitflyerNormalizedApi normalized) =>
        new(normalized);

    private static ISpotHistoryApi CreateSpotHistoryApi(IBitflyerNormalizedApi normalized) =>
        new BitflyerSpotHistoryApi(normalized);

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

    // Raw access removed from public facade.
}
