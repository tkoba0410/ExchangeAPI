using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;
using Common.Contract.Interfaces;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Common.Contract.Errors;
using Common.Transport.Protocol;

namespace ExchangeApi.Adapter.Bittrade.Apis;

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

    public async Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(symbol);
        var response = await _restClient.GetAsync<BittradeMergedResponse>(
            $"market/detail/merged?symbol={apiSymbol}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick is null)
        {
            throw new ExchangeApiException("Bittrade ticker response is invalid.");
        }

        var tick = response.Tick;
        var ts = response.Ts ?? tick.Ts;
        var timestamp = ts.HasValue && ts.Value > 0
            ? DateTimeOffset.FromUnixTimeMilliseconds(ts.Value)
            : DateTimeOffset.UtcNow;
        var canonicalSymbol = ToCanonicalSymbol(symbol);
        var symbolEnum = MapSymbol(canonicalSymbol);

        return new Ticker(
            Exchange: ExchangeCode.Bittrade,
            Symbol: symbolEnum,
            LastTradedPrice: tick.Close,
            Timestamp: timestamp);
    }

    public async Task<OrderBook> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(symbol);
        var response = await _restClient.GetAsync<BittradeDepthResponse>(
            $"market/depth?symbol={apiSymbol}&type=step0",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick is null)
        {
            throw new ExchangeApiException("Bittrade depth response is invalid.");
        }

        var bids = response.Tick.Bids?.Select(ToLevel).ToList() ?? new List<OrderBookLevel>();
        var asks = response.Tick.Asks?.Select(ToLevel).ToList() ?? new List<OrderBookLevel>();

        return new OrderBook(bids, asks);
    }

    public async Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var apiSymbol = ToApiSymbol(symbol);
        var response = await _restClient.GetAsync<BittradeTradeResponse>(
            $"market/trade?symbol={apiSymbol}",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Tick?.Data is null)
        {
            throw new ExchangeApiException("Bittrade trades response is invalid.");
        }

        var productCode = ToCanonicalSymbol(symbol);
        var executions = response.Tick.Data
            .Select(d => new ExecutionMarket(
                productCode,
                d.Id,
                MapSide(d.Direction),
                d.Price,
                d.Amount,
                DateTimeOffset.FromUnixTimeMilliseconds(d.Ts)))
            .ToList();

        return executions;
    }

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        string symbol,
        string timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Bittrade candlesticks are not implemented.");
    }

    private static OrderBookLevel ToLevel(IReadOnlyList<decimal> level)
    {
        if (level.Count < 2) throw new ExchangeApiException("Invalid order book level.");
        return new OrderBookLevel(level[0], level[1]);
    }

    private static OrderSide MapSide(string direction) =>
        string.Equals(direction, "buy", StringComparison.OrdinalIgnoreCase)
            ? OrderSide.Buy
            : OrderSide.Sell;

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static string ToCanonicalSymbol(string symbol)
    {
        if (symbol.Contains('/')) return symbol.ToUpperInvariant();
        var upper = symbol.ToUpperInvariant();
        if (upper.EndsWith("JPY", StringComparison.Ordinal))
        {
            var basePart = upper[..^3];
            return $"{basePart}/JPY";
        }

        // fallback: split midpoint
        if (upper.Length >= 6)
        {
            var mid = upper.Length / 2;
            return $"{upper[..mid]}/{upper[mid..]}";
        }

        return upper;
    }

    private static Symbol MapSymbol(string canonicalSymbol) =>
        canonicalSymbol switch
        {
            "BTC/JPY" or "BTC_JPY" => Symbol.BtcJpy,
            "ETH/JPY" or "ETH_JPY" => Symbol.EthJpy,
            "FX_BTC/JPY" or "FX_BTC_JPY" => Symbol.FxBtcJpy,
            _ => Symbol.Unknown
        };
}
