using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class PublicClient : IContractPublicClient, IContractCandlesticksClient, IDisposable
{
    private readonly MarketApi _marketApi;
    private IDisposable? _ownedDisposable;

    public PublicClient(ClientOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var (components, restClient) = BittradeClientBootstrap.CreatePublicComponents(options);
        _marketApi = new MarketApi(components.Public, components.Markets);
        _ownedDisposable = restClient;
    }

    public PublicClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        // 公開APIのみ: market 取得に限定し、Trading/Account/History は提供しない。
        var components = BittradeClientComponents.FromRestClient(restClient, accountId: null);
        _marketApi = new MarketApi(components.Public, components.Markets);
    }

    internal PublicClient(BittradeClientComponents components, IDisposable? ownedDisposable = null)
    {
        if (components is null) throw new ArgumentNullException(nameof(components));
        _marketApi = new MarketApi(components.Public, components.Markets);
        _ownedDisposable = ownedDisposable;
    }

    public Task<Call<TickerRequest, TickerResponse>> GetTickerAsync(
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

    public Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        CandlesticksRequest request,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(request, cancellationToken);

    public void Dispose()
    {
        _ownedDisposable?.Dispose();
        _ownedDisposable = null;
    }

    // Raw access removed from public facade.
}
