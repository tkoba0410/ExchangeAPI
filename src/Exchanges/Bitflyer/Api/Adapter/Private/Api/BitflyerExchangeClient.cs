using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
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
    public Task<Call<TickerRequest, CommonTicker>> GetTickerAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardAsync(symbol, cancellationToken);

    public Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetExecutionsPublicAsync(symbol, cancellationToken);

    public Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        Symbol symbol,
        PeriodDto period,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, period, size, cancellationToken);

    // ExchangeInfo
    public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    // Trading
    public Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        _tradingApi.OrderLimitAsync(symbol, side, size, price, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderAsync(symbol, orderKey, cancellationToken);

    // Account
    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        CancellationToken cancellationToken = default) =>
        _accountApi.GetBalanceAsync(cancellationToken);

    // SpotHistory
    public Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        OrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _historyApi.GetOrdersAsync(request, cancellationToken);

    public Task<Call<ExecutionsPrivateRequest, ExecutionsPrivateResponse>> GetExecutionsPrivateAsync(
        ExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default) =>
        _historyApi.GetExecutionsPrivateAsync(request, cancellationToken);

    // Raw access removed from public facade.
}
