using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.Ticker;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BitflyerExchangeClient : IPublicApi, IPrivateApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly BitflyerTradingApi _tradingApi;
    private readonly BitflyerAccountApi _accountApi;
    private readonly BitflyerSpotHistoryApi _historyApi;
    private readonly BitflyerExchangeInfoApi _exchangeInfoApi;
    internal BitflyerApiBundle? ApiBundle { get; }
    // IExchangeClient (nullable capability) に合わせる。実体は常に non-null。
    public IPublicApi? Public => this;
    public IPrivateApi? Private => this;

    internal BitflyerExchangeClient(
        IBitflyerNormalizedApi normalized,
        object? rawBundle = null)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        var exchangeInfo = new BitflyerExchangeInfoApi(normalized);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        _marketApi = new MarketApi(normalized, markets);
        _tradingApi = new BitflyerTradingApi(normalized);
        _accountApi = new BitflyerAccountApi(normalized);
        _historyApi = new BitflyerSpotHistoryApi(normalized);
        _exchangeInfoApi = exchangeInfo;
    }

    internal BitflyerExchangeClient(
        MarketApi marketApi,
        BitflyerTradingApi tradingApi,
        BitflyerAccountApi accountApi,
        BitflyerSpotHistoryApi historyApi,
        BitflyerExchangeInfoApi exchangeInfoApi,
        object? rawBundle = null)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _historyApi = historyApi ?? throw new ArgumentNullException(nameof(historyApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
    }

    internal BitflyerExchangeClient(BitflyerApiBundle bundle)
        : this(
            marketApi: new MarketApi(bundle.Normalized, bundle.Markets),
            tradingApi: new BitflyerTradingApi(bundle.Normalized),
            accountApi: new BitflyerAccountApi(bundle.Normalized),
            historyApi: new BitflyerSpotHistoryApi(bundle.Normalized),
            exchangeInfoApi: bundle.ExchangeInfo)
    {
        ApiBundle = bundle;
    }

    public static BitflyerExchangeClient FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var bundle = BitflyerApiBundle.FromRestClient(restClient);
        return new BitflyerExchangeClient(bundle);
    }

    // Market
    public Task<Call<GetTickerRequest, CommonTicker>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetBoardCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardCallAsync(symbol, cancellationToken);

    public Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetExecutionsPublicCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetExecutionsPublicCallAsync(symbol, cancellationToken);

    // ExchangeInfo
    public Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoCallAsync(cancellationToken);

    // Trading
    public Task<Call<PlaceLimitOrderRequest, OrderResult>> OrderLimitCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        _tradingApi.OrderLimitCallAsync(symbol, side, size, price, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderCallAsync(symbol, orderKey, cancellationToken);

    // Account
    public Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _accountApi.GetBalanceCallAsync(cancellationToken);

    // SpotHistory
    public Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default) =>
        _historyApi.GetOrdersCallAsync(request, cancellationToken);

    public Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsPrivateCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default) =>
        _historyApi.GetExecutionsPrivateCallAsync(request, cancellationToken);

    // Raw access removed from public facade.
}
