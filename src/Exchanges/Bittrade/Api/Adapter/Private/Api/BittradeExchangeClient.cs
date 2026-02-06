using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;

/// <summary>
/// Bittrade 用のファサード。各 API 実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class BittradeExchangeClient : IPublicApi, IPrivateApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly BittradeTradingApi _tradingApi;
    private readonly BittradeAccountApi _accountApi;
    private readonly BittradeSpotHistoryApi _historyApi;
    private readonly BittradeExchangeInfoApi _exchangeInfoApi;
    private readonly IRestClient? _restClient;
    internal BittradeApiBundle? ApiBundle { get; }

    // IExchangeClient (nullable capability) に合わせる。実体は常に non-null。
    public IPublicApi? Public => this;
    public IPrivateApi? Private => this;

    internal BittradeExchangeClient(
        MarketApi marketApi,
        BittradeTradingApi tradingApi,
        BittradeAccountApi accountApi,
        BittradeSpotHistoryApi historyApi,
        BittradeExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _historyApi = historyApi ?? throw new ArgumentNullException(nameof(historyApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
    }

    internal BittradeExchangeClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        if (bundle.AccountId is null || bundle.AccountId.Value.IsEmpty)
        {
            throw new InvalidOperationException("BittradeApiBundle.AccountId is required to create BittradeExchangeClient.");
        }
        if (bundle.Private is null)
        {
            throw new InvalidOperationException("BittradeApiBundle.Private is required to create BittradeExchangeClient.");
        }

        _marketApi = new MarketApi(bundle.Public, bundle.Markets);
        _tradingApi = new BittradeTradingApi(bundle.Private);
        _accountApi = new BittradeAccountApi(bundle.Private);
        _historyApi = new BittradeSpotHistoryApi(bundle.Private);
        _exchangeInfoApi = new BittradeExchangeInfoApi(bundle.Public);
        _restClient = bundle.RestClient;
        ApiBundle = bundle;
    }

    internal BittradeExchangeClient(
        MarketApi marketApi,
        BittradeTradingApi tradingApi,
        BittradeAccountApi accountApi,
        BittradeSpotHistoryApi historyApi,
        BittradeExchangeInfoApi exchangeInfoApi,
        IRestClient restClient)
        : this(marketApi, tradingApi, accountApi, historyApi, exchangeInfoApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    internal BittradeExchangeClient(
        MarketApi marketApi,
        BittradeTradingApi tradingApi,
        BittradeAccountApi accountApi,
        BittradeSpotHistoryApi historyApi,
        BittradeExchangeInfoApi exchangeInfoApi,
        IRestClient restClient,
        string accountId)
        : this(marketApi, tradingApi, accountApi, historyApi, exchangeInfoApi)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<Call<TickerRequest, TickerResponse>> GetTickerAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardAsync(symbol, cancellationToken);

    public Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetExecutionsPublicAsync(symbol, cancellationToken);

    // ExchangeInfo
    public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        CancellationToken cancellationToken = default) =>
        _accountApi.GetBalanceAsync(cancellationToken);

    public Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        _tradingApi.OrderLimitAsync(symbol, side, size, price, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderAsync(symbol, orderKey, cancellationToken);

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
