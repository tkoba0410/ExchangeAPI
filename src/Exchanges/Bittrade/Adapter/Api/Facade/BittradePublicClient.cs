using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Shared.Adapter.NotSupported;
using CommonSymbol = ExchangeApi.Contracts.Common.DomainCommon.Types.Symbol;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Market;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Contracts.Common.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Facade;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BittradePublicClient : IMarketDataApi, IExchangeClient, IHasRawAccess
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly ISpotHistoryApi _historyApi;
    private readonly IRestClient? _restClient;
    private readonly object? _rawBundle;
    internal BittradeApiBundle? ApiBundle { get; }

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bittrade;
    public IMarketDataApi Market => _marketApi;
    public ITradingApi Trading => _tradingApi;
    public IAccountApi Account => _accountApi;
    public ISpotHistoryApi History => _historyApi;

    public BittradePublicClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizeBundle.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        _marketApi = new BittradeMarketDataApi(normalizeBundle.MarketData, markets);
        _tradingApi = new NotSupportedTradingApi(ExchangeCode.Bittrade);
        _accountApi = new NotSupportedAccountApi(ExchangeCode.Bittrade);
        _historyApi = new NotSupportedSpotHistoryApi();
        _restClient = restClient;
        _rawBundle = normalizeBundle.RawBundle;
    }

    public BittradePublicClient(IMarketDataApi marketApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = new NotSupportedTradingApi(ExchangeCode.Bittrade);
        _accountApi = new NotSupportedAccountApi(ExchangeCode.Bittrade);
        _historyApi = new NotSupportedSpotHistoryApi();
    }

    internal BittradePublicClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        _marketApi = new BittradeMarketDataApi(bundle.NormalizedMarketData, bundle.Markets);
        _tradingApi = new NotSupportedTradingApi(ExchangeCode.Bittrade);
        _accountApi = new NotSupportedAccountApi(ExchangeCode.Bittrade);
        _historyApi = new NotSupportedSpotHistoryApi();
        _restClient = bundle.RestClient;
        _rawBundle = bundle.RawBundle;
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

    public bool TryGetRaw<T>(out T raw) where T : class
    {
        raw = _rawBundle as T ?? null!;
        return raw is not null;
    }
}
