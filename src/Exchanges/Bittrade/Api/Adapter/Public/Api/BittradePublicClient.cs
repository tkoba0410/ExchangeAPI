using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfo.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Requests;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Internal;
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

    public Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookCallAsync(symbol, cancellationToken);

    public Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsCallAsync(symbol, cancellationToken);

    public Task<Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>> GetHistoryKlineCallAsync(
        CommonSymbol symbol,
        string period,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetHistoryKlineCallAsync(symbol, period, size, cancellationToken);

    public Task<Call<GetTickersRequest, IReadOnlyList<Ticker>>> GetTickersCallAsync(
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickersCallAsync(cancellationToken);

    public Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>> GetHistoryTradeCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetHistoryTradeCallAsync(symbol, cancellationToken);

    public Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoCallAsync(cancellationToken);

    public Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetCurrencysCallAsync(cancellationToken);

    public Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetTimestampCallAsync(cancellationToken);

    // Raw access removed from public facade.
}
