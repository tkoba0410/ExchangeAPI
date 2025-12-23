using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire.Public.Models;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public;

internal sealed class BittradeWireMarketDataApi : IBittradeWireMarketDataApi
{
    private const string OkStatus = "ok";
    private readonly IBittradeRawMarketDataApi _raw;

    public BittradeWireMarketDataApi(IBittradeRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<BittradeWireTicker> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        var raw = await _raw.GetTickerAsync(Symbol.From(symbol), ct).ConfigureAwait(false);
        RequireOk(raw.Status, operation: "Bittrade.Wire.MarketData.GetTicker");

        var tick = raw.Tick ?? throw Missing("tick", "Bittrade.Wire.MarketData.GetTicker");
        var bestBid = GetBestPrice(tick.Bid, "bid", "Bittrade.Wire.MarketData.GetTicker");
        var bestAsk = GetBestPrice(tick.Ask, "ask", "Bittrade.Wire.MarketData.GetTicker");
        var timestamp = tick.Ts ?? raw.Ts ?? throw Missing("ts", "Bittrade.Wire.MarketData.GetTicker");

        return new BittradeWireTicker(
            BestBid: bestBid,
            BestAsk: bestAsk,
            Last: tick.Close,
            Volume: tick.Volume,
            Timestamp: timestamp);
    }

    public async Task<BittradeWireOrderBook> GetOrderBookAsync(string symbol, CancellationToken ct = default)
    {
        var raw = await _raw.GetOrderBookAsync(Symbol.From(symbol), cancellationToken: ct).ConfigureAwait(false);
        RequireOk(raw.Status, operation: "Bittrade.Wire.MarketData.GetOrderBook");

        var tick = raw.Tick ?? throw Missing("tick", "Bittrade.Wire.MarketData.GetOrderBook");
        var bids = MapLevels(tick.Bids, "bids", "Bittrade.Wire.MarketData.GetOrderBook");
        var asks = MapLevels(tick.Asks, "asks", "Bittrade.Wire.MarketData.GetOrderBook");

        return new BittradeWireOrderBook(bids, asks);
    }

    private static void RequireOk(string? status, string operation)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ExchangeApiException(
                $"{operation}: status is missing.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
        }

        if (!string.Equals(status, OkStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new ExchangeApiException(
                $"{operation}: status={status}.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Request);
        }
    }

    private static decimal GetBestPrice(decimal[] values, string field, string operation)
    {
        if (values.Length < 2)
        {
            throw new ExchangeApiException(
                $"{operation}: {field} is invalid.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
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
            throw new ExchangeApiException(
                $"{operation}: {field} is missing.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
        }

        return levels.Select(level => MapLevel(level, field, operation)).ToArray();
    }

    private static BittradeWirePriceSize MapLevel(IReadOnlyList<decimal> level, string field, string operation)
    {
        if (level.Count < 2)
        {
            throw new ExchangeApiException(
                $"{operation}: {field} has invalid level.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
        }

        return new BittradeWirePriceSize(level[0], level[1]);
    }

    private static ExchangeApiException Missing(string field, string operation) =>
        new($"{operation}: missing {field}.", exchange: ExchangeCode.Bittrade, operation: operation, errorCategory: ExchangeErrorCategory.Unknown);
}
