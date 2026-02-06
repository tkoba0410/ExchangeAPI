using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BittradePublicClient : IPublicApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly BittradeExchangeInfoApi _exchangeInfoApi;
    private readonly IRestClient? _restClient;
    internal BittradeApiBundle? ApiBundle { get; }

    public IPublicApi? Public => this;
    public IPrivateApi? Private => null;

    public BittradePublicClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        // 公開APIのみ: market/exchangeInfo 取得に限定し、Trading/Account/History は提供しない。
        var bundle = BittradeApiBundle.FromRestClient(restClient, accountId: null);
        _marketApi = new MarketApi(bundle.Public, bundle.Markets);
        _exchangeInfoApi = new BittradeExchangeInfoApi(bundle.Public);
        _restClient = restClient;
    }

    internal BittradePublicClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        _marketApi = new MarketApi(bundle.Public, bundle.Markets);
        _exchangeInfoApi = new BittradeExchangeInfoApi(bundle.Public);
        _restClient = bundle.RestClient;
        ApiBundle = bundle;
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

    public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    // Raw access removed from public facade.
}
