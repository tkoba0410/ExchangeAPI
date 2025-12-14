using System;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using Common.Contract.Interfaces;
using Common.Contract.Dtos;
using Common.Contract.Enums;

namespace Exchange.Bitflyer.Abstract;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BitflyerPublicClient : IMarketDataApi, IExchangeInfoApi
{
    private readonly IMarketDataApi _marketApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;

    public BitflyerPublicClient(IBitflyerPublicApi publicApi)
    {
        if (publicApi is null) throw new ArgumentNullException(nameof(publicApi));
        _marketApi = new BitflyerMarketApi(publicApi, "bitFlyer");
        _exchangeInfoApi = new BitflyerExchangeInfoApi();
    }

    public Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(string symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(string symbol, string timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);
}
