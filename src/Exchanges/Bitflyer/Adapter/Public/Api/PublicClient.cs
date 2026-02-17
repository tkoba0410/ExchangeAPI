using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class PublicClient : IPublicApi, IExchangeClient
{
    private readonly MarketApi _marketApi;

    public IPublicApi? Public => this;
    public IPrivateApi? Private => null;
    public ICandlesticksApi? Candlesticks => null;

    public PublicClient(
        ClientOptions options,
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var client = ClientFactory.CreatePublic(options, httpClient, transportOverride);
        _marketApi = client._marketApi;
    }

    internal PublicClient(INormalizedApi normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _marketApi = new MarketApi(normalized, new BitflyerMarketCatalogResolver());
    }

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

    // Raw access removed from public facade.
}
