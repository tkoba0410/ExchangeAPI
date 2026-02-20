using System;
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
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class ExchangeClient : IContractPrivateClient, IDisposable
{
    private readonly MarketApi _marketApi;
    private readonly PrivateApi _privateApi;
    private IDisposable? _ownedDisposable;

    public ExchangeClient(
        ClientOptions options,
        ClientCredentials credentials)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (credentials is null) throw new ArgumentNullException(nameof(credentials));

        var client = ClientFactory.Create(credentials, options);
        _marketApi = client._marketApi;
        _privateApi = client._privateApi;
        _ownedDisposable = client._ownedDisposable;
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
        IExchangeMarketResolver markets,
        IDisposable? ownedDisposable = null)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        if (markets is null) throw new ArgumentNullException(nameof(markets));
        _marketApi = new MarketApi(normalized, markets);
        _privateApi = new PrivateApi(normalized);
        _ownedDisposable = ownedDisposable;
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

    public void Dispose()
    {
        _ownedDisposable?.Dispose();
        _ownedDisposable = null;
    }

    // Raw access removed from public facade.
}
