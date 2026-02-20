using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Resolve;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Bootstrap;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Orchestration;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class PublicClient : IContractPublicClient, IDisposable
{
    private readonly PublicFlow _publicFlow;
    private IDisposable? _ownedDisposable;

    public PublicClient(ClientOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var client = ClientFactory.CreatePublic(options);
        _publicFlow = client._publicFlow;
        _ownedDisposable = client._ownedDisposable;
    }

    internal PublicClient(INormalizedApi normalized, IDisposable? ownedDisposable = null)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _publicFlow = new PublicFlow(normalized, new ExchangeRequestResolver());
        _ownedDisposable = ownedDisposable;
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

    public void Dispose()
    {
        _ownedDisposable?.Dispose();
        _ownedDisposable = null;
    }

    // Raw access removed from public facade.
}
