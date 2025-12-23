using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire.Public.Models;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public;

internal sealed class BittradeWireMarketDataApi : IBittradeWireMarketDataApi
{
    private readonly IBittradeRawMarketDataApi _raw;

    public BittradeWireMarketDataApi(IBittradeRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<BittradeWireTicker> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.MarketData.GetTicker;
        var raw = await _raw.GetTickerAsync(Symbol.From(symbol), ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);

        var tick = raw.Tick ?? throw BittradeWireErrors.Missing(operation, "tick");
        var bestBid = GetBestPrice(tick.Bid, "bid", operation);
        var bestAsk = GetBestPrice(tick.Ask, "ask", operation);
        var timestamp = tick.Ts ?? raw.Ts ?? throw BittradeWireErrors.Missing(operation, "ts");

        return new BittradeWireTicker(
            BestBid: bestBid,
            BestAsk: bestAsk,
            Last: tick.Close,
            Volume: tick.Volume,
            Timestamp: timestamp);
    }

    public async Task<BittradeWireOrderBook> GetOrderBookAsync(string symbol, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.MarketData.GetOrderBook;
        var raw = await _raw.GetOrderBookAsync(Symbol.From(symbol), cancellationToken: ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);

        var tick = raw.Tick ?? throw BittradeWireErrors.Missing(operation, "tick");
        var bids = MapLevels(tick.Bids, "bids", operation);
        var asks = MapLevels(tick.Asks, "asks", operation);

        return new BittradeWireOrderBook(bids, asks);
    }

    private static decimal GetBestPrice(decimal[] values, string field, string operation)
    {
        if (values.Length < 2)
        {
            throw BittradeWireErrors.Unexpected(operation, field, "invalid-level");
        }

        return values[0];
    }

    private static IReadOnlyList<BittradeWirePriceSize> MapLevels(
        IReadOnlyList<IReadOnlyList<decimal>>? levels,
        string field,
        string operation)
    {
        if (levels is null)
        {
            throw BittradeWireErrors.Missing(operation, field);
        }

        return levels.Select(level => MapLevel(level, field, operation)).ToArray();
    }

    private static BittradeWirePriceSize MapLevel(IReadOnlyList<decimal> level, string field, string operation)
    {
        if (level.Count < 2)
        {
            throw BittradeWireErrors.Unexpected(operation, field, "invalid-level");
        }

        return new BittradeWirePriceSize(level[0], level[1]);
    }
}
