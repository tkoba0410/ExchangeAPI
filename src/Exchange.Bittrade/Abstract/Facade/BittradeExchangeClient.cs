using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.Apis;
using ExchangeApi.Adapter.Bittrade.Apis.ExchangeInfo;
using Common.Contract.Interfaces;
using Common.Contract.Dtos;

namespace ExchangeApi.Adapter.Bittrade.Facade;

/// <summary>
/// Bittrade 用のファサード。Raw API も提供する。
/// </summary>
public sealed class BittradeExchangeClient : IMarketDataApi, ITradingApi, IAccountApi, IExchangeInfoApi
{
    private readonly IMarketDataApi _marketApi;
    private readonly ITradingApi _tradingApi;
    private readonly IAccountApi _accountApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;

    public BittradeRawApiClient Raw { get; }

    public BittradeExchangeClient(
        IMarketDataApi marketApi,
        ITradingApi tradingApi,
        IAccountApi accountApi,
        IExchangeInfoApi exchangeInfoApi,
        BittradeRawApiClient raw)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
        Raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<MarketExecution>> GetMarketExecutionsAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(string symbol, string timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _accountApi.GetBalancesAsync(cancellationToken);

    public Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken cancellationToken = default) =>
        _tradingApi.SendOrderAsync(request, cancellationToken);

    public Task<CancelResult> CancelOrderAsync(string productCode, string orderId, CancellationToken cancellationToken = default) =>
        _tradingApi.CancelOrderAsync(productCode, orderId, cancellationToken);

    public Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(string productCode, CancellationToken cancellationToken = default) =>
        _tradingApi.GetOpenOrdersAsync(productCode, cancellationToken);

    public Task<OrderStatus> PollOrderStatusAsync(string productCode, string orderAcceptanceId, TimeSpan? pollInterval = null, int maxAttempts = 30, CancellationToken cancellationToken = default) =>
        _tradingApi.PollOrderStatusAsync(productCode, orderAcceptanceId, pollInterval, maxAttempts, cancellationToken);

    public Task<IReadOnlyList<AccountExecution>> GetAccountExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _accountApi.GetAccountExecutionsAsync(productCode, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);
}
