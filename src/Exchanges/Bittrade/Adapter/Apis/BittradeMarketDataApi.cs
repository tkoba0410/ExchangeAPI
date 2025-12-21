using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis;

/// <summary>
/// Bittrade の Public REST 実装（Ticker/OrderBook/Executions）。
/// </summary>
public sealed class BittradeMarketDataApi : IMarketDataApi
{
    private readonly IRestClient _restClient;

    public BittradeMarketDataApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<TimestampResponse>("v1/common/timestamp", cancellationToken: cancellationToken);

    public async Task<Ticker> GetTickerAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(symbol);
        var response = await _restClient.GetAsync<MergedResponse>(
            $"market/detail/merged?symbol={apiSymbol}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick is null)
        {
            throw new ExchangeApiException("Bittrade ticker response is invalid.");
        }

        var tick = response.Tick;
        var ts = response.Ts ?? tick.Ts;
        var timestamp = ts ?? DateTimeOffset.UtcNow;
        return new Ticker(
            Exchange: ExchangeCode.Bittrade,
            Symbol: symbol,
            LastTradedPrice: tick.Close,
            Timestamp: timestamp);
    }

    public async Task<OrderBook> GetOrderBookAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(symbol);
        var response = await _restClient.GetAsync<DepthResponse>(
            $"market/depth?symbol={apiSymbol}&type=step0",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick is null)
        {
            throw new ExchangeApiException("Bittrade depth response is invalid.");
        }

        var bids = response.Tick.Bids?.Select(ToLevel).ToList() ?? new List<OrderBookLevel>();
        var asks = response.Tick.Asks?.Select(ToLevel).ToList() ?? new List<OrderBookLevel>();

        return new OrderBook(ExchangeCode.Bittrade, bids, asks);
    }

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(symbol);
        var response = await _restClient.GetAsync<TradeResponse>(
            $"market/trade?symbol={apiSymbol}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick?.Data is null)
        {
            throw new ExchangeApiException("Bittrade trades response is invalid.");
        }

        var executions = response.Tick.Data
            .Select(d => new ExecutionMarket(
                ExchangeCode.Bittrade,
                symbol,
                d.Id.ToString(),
                MapSide(d.Direction),
                d.Price,
                d.Amount,
                d.Ts))
            .ToList();

        return executions;
    }

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        CommonSymbol symbol,
        TimeSpan timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Candlesticks");
    }

    private static OrderBookLevel ToLevel(IReadOnlyList<decimal> level)
    {
        if (level.Count < 2) throw new ExchangeApiException("Invalid order book level.");
        return new OrderBookLevel(level[0], level[1]);
    }

    private static Side MapSide(string direction) =>
        string.Equals(direction, "buy", StringComparison.OrdinalIgnoreCase)
            ? Side.Buy
            : Side.Sell;

    private static string ToApiSymbol(CommonSymbol symbol) =>
        BittradeSymbolMapper.ToApiSymbol(symbol);
}
