using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Resolve;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Orchestration;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Primitives.CallCommon;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

/// <summary>
/// bitFlyer 用のファサード。各API実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class ExchangeClient : IContractPrivateClient, IDisposable
{
    private readonly PublicFlow _publicFlow;
    private readonly PrivateFlow _privateFlow;
    private IDisposable? _ownedDisposable;

    public ExchangeClient(
        ClientOptions options,
        ClientCredentials credentials)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (credentials is null) throw new ArgumentNullException(nameof(credentials));

        var client = ClientFactory.Create(credentials, options);
        _publicFlow = client._publicFlow;
        _privateFlow = client._privateFlow;
        _ownedDisposable = client._ownedDisposable;
    }

    internal ExchangeClient(
        INormalizedApi normalized,
        object? rawBundle = null)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _publicFlow = new PublicFlow(normalized, new ExchangeRequestResolver());
        _privateFlow = new PrivateFlow(normalized);
    }

    internal ExchangeClient(
        INormalizedApi normalized,
        IExchangeMarketResolver markets,
        IDisposable? ownedDisposable = null)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        if (markets is null) throw new ArgumentNullException(nameof(markets));
        _publicFlow = new PublicFlow(normalized, markets);
        _privateFlow = new PrivateFlow(normalized);
        _ownedDisposable = ownedDisposable;
    }

    public static ExchangeClient FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var components = BitflyerClientComponents.FromRestClient(restClient);
        return new ExchangeClient(components.Normalized, components.Markets);
    }

    public Task<Call<TickerRequest, CommonTicker>> GetTickerAsync(
        TickerRequest request,
        CancellationToken cancellationToken = default) =>
        _publicFlow.GetTickerAsync(request, cancellationToken);

    public Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        BoardRequest request,
        CancellationToken cancellationToken = default) =>
        _publicFlow.GetBoardAsync(request, cancellationToken);

    public Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        ExecutionsPublicRequest request,
        CancellationToken cancellationToken = default) =>
        _publicFlow.GetExecutionsPublicAsync(request, cancellationToken);

    public Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        OrderLimitRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.OrderLimitAsync(request, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.CancelOrderAsync(request, cancellationToken);

    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.GetBalanceAsync(request, cancellationToken);

    public Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        OrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.GetOrdersAsync(request, cancellationToken);

    public Task<Call<ExecutionsPrivateRequest, ExecutionsPrivateResponse>> GetExecutionsPrivateAsync(
        ExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.GetExecutionsPrivateAsync(request, cancellationToken);

    public void Dispose()
    {
        _ownedDisposable?.Dispose();
        _ownedDisposable = null;
    }

    // Raw access removed from public facade.
}
