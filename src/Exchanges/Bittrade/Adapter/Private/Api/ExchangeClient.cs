using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Orchestration;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade 用のファサード。各 API 実装を委譲するだけの薄いラッパー。
/// </summary>
public sealed class ExchangeClient : IContractPrivateClient, IContractCandlesticksClient, IDisposable
{
    private readonly PublicFlow _publicFlow;
    private readonly PrivateFlow _privateFlow;
    private IDisposable? _ownedDisposable;

    public ExchangeClient(
        ClientOptions options,
        ClientCredentials credentials,
        string accountId = "default")
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (credentials is null) throw new ArgumentNullException(nameof(credentials));
        var (components, _, restClient) = BittradeClientBootstrap.CreatePrivateComponents(options, credentials, accountId);
        if (components.Private is null)
        {
            throw new InvalidOperationException("Private components are required to create ExchangeClient.");
        }

        _publicFlow = new PublicFlow(components.Public, components.Markets);
        _privateFlow = new PrivateFlow(components.Private);
        _ownedDisposable = restClient;
    }

    internal ExchangeClient(
        PublicFlow publicFlow,
        PrivateFlow privateFlow)
    {
        _publicFlow = publicFlow ?? throw new ArgumentNullException(nameof(publicFlow));
        _privateFlow = privateFlow ?? throw new ArgumentNullException(nameof(privateFlow));
    }

    internal ExchangeClient(BittradeClientComponents components, AccountId accountId, IDisposable? ownedDisposable = null)
    {
        if (components is null) throw new ArgumentNullException(nameof(components));
        if (accountId.IsEmpty) throw new ArgumentException("accountId is required.", nameof(accountId));
        if (components.Private is null)
        {
            throw new InvalidOperationException("Private components are required to create ExchangeClient.");
        }

        _publicFlow = new PublicFlow(components.Public, components.Markets);
        _privateFlow = new PrivateFlow(components.Private);
        _ownedDisposable = ownedDisposable;
    }

    internal ExchangeClient(
        PublicFlow publicFlow,
        PrivateFlow privateFlow,
        IRestClient restClient)
        : this(publicFlow, privateFlow)
    {
        _ownedDisposable = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public static ExchangeClient FromRestClient(IRestClient restClient, AccountId accountId)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (accountId.IsEmpty) throw new ArgumentException("accountId is required.", nameof(accountId));
        var components = BittradeClientComponents.FromRestClient(restClient, accountId);
        return new ExchangeClient(components, accountId);
    }

    public Task<Call<TickerRequest, TickerResponse>> GetTickerAsync(
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

    public Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        CandlesticksRequest request,
        CancellationToken cancellationToken = default) =>
        _publicFlow.GetCandlesticksAsync(request, cancellationToken);

    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.GetBalanceAsync(request, cancellationToken);

    public Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        OrderLimitRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.OrderLimitAsync(request, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateFlow.CancelOrderAsync(request, cancellationToken);

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
