using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Http;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class ExchangeClient : IPublicApi, IPrivateApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly PrivateApi _privateApi;
    // IExchangeClient (nullable capability) に合わせる。実体は常に non-null。
    public IPublicApi? Public => this;
    public IPrivateApi? Private => this;
    public ICandlesticksApi? Candlesticks => null;

    public ExchangeClient(
        ClientOptions options,
        ClientCredentials credentials,
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (credentials is null) throw new ArgumentNullException(nameof(credentials));

        var client = ClientFactory.Create(credentials, options, httpClient, transportOverride);
        _marketApi = client._marketApi;
        _privateApi = client._privateApi;
    }

    internal ExchangeClient(
        INormalizedApi normalized,
        object? rawBundle = null)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _marketApi = new MarketApi(normalized, new BitflyerMarketCatalogResolver());
        _privateApi = new PrivateApi(normalized);
    }

    internal ExchangeClient(
        INormalizedApi normalized,
        IExchangeMarketResolver markets)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        if (markets is null) throw new ArgumentNullException(nameof(markets));
        _marketApi = new MarketApi(normalized, markets);
        _privateApi = new PrivateApi(normalized);
    }

    public static ExchangeClient FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var components = BitflyerClientComponents.FromRestClient(restClient);
        return new ExchangeClient(components.Normalized, components.Markets);
    }

    // Market
    public Task<Call<TickerRequest, CommonTicker>> GetTickerAsync(
        TickerRequest request,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(request, cancellationToken);

    public Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        BoardRequest request,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardAsync(request, cancellationToken);

    public Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        ExecutionsPublicRequest request,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetExecutionsPublicAsync(request, cancellationToken);

    // Trading
    public Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        OrderLimitRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.OrderLimitAsync(request, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelOrderAsync(request, cancellationToken);

    // Account
    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceAsync(request, cancellationToken);

    // SpotHistory
    public Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        OrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersAsync(request, cancellationToken);

    public Task<Call<ExecutionsPrivateRequest, ExecutionsPrivateResponse>> GetExecutionsPrivateAsync(
        ExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetExecutionsPrivateAsync(request, cancellationToken);

    // Raw access removed from public facade.
}
