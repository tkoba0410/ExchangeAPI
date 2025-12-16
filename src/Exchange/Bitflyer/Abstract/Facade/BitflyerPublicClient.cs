using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Dtos;
using Common.Enums;
using Exchange.Bitflyer.Abstract.Apis.ExchangeInfo;
using Exchange.Bitflyer.Abstract.Apis.Market;
using Exchange.Bitflyer.Raw;
namespace Exchange.Bitflyer.Abstract.Facade;

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

    public Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(Symbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);
}
