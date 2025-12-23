using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Clients.Internal;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Market;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using CommonTicker = ExchangeApi.Common.Dtos.Ticker;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BitflyerPublicClient : IMarketDataApi, IExchangeInfoApi, IExchangeClient, IHasRawAccess, IHasWireAccess
{
    private readonly MarketApi _marketApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    private readonly object? _rawBundle;
    private readonly object? _wireBundle;

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bitflyer;

    internal BitflyerPublicClient(IBitflyerPublicApi publicApi, object? rawBundle = null, object? wireBundle = null)
    {
        if (publicApi is null) throw new ArgumentNullException(nameof(publicApi));
        _marketApi = new MarketApi(publicApi, ExchangeCode.Bitflyer);
        _exchangeInfoApi = new BitflyerExchangeInfoApi();
        _rawBundle = rawBundle;
        _wireBundle = wireBundle;
    }

    public Task<CommonTicker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(Symbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<HealthResponse> GetHealthAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetHealthAsync(symbol, cancellationToken);

    public Task<BoardStateResponse> GetBoardStateAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardStateAsync(symbol, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    public bool TryGetRaw<T>(out T raw) where T : class
    {
        raw = _rawBundle as T ?? null!;
        return raw is not null;
    }

    public bool TryGetWire<T>(out T wire) where T : class
    {
        wire = _wireBundle as T ?? null!;
        return wire is not null;
    }
}
