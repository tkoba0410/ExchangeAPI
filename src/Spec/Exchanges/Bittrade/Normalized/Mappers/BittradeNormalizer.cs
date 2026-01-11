using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;

internal static class BittradeNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    internal static BittradeTickerNormalized NormalizeTicker(RawMergedResponse response, string? rawJson)
    {
        if (response is null) throw new ArgumentNullException(nameof(response));

        var tick = response.Tick ?? throw new ArgumentNullException(nameof(response.Tick));
        var timestamp = tick.Ts ?? response.Ts ?? DateTimeOffset.UtcNow;
        var snapshot = ExtractSnapshot(rawJson ?? Serialize(response));
        return new BittradeTickerNormalized(
            tick.Close,
            timestamp,
            snapshot,
            new Dictionary<string, JsonElement>());
    }

    internal static BittradeOrderBookNormalized NormalizeOrderBook(RawDepthTick tick)
    {
        if (tick is null) throw new ArgumentNullException(nameof(tick));

        var bids = NormalizeLevels(tick.Bids, "bids");
        var asks = NormalizeLevels(tick.Asks, "asks");
        return new BittradeOrderBookNormalized(bids, asks);
    }

    internal static IReadOnlyList<BittradeExecutionNormalized> NormalizeExecutions(
        IReadOnlyList<RawTradeEntry> entries,
        string? rawJson)
    {
        if (entries is null) throw new ArgumentNullException(nameof(entries));

        var snapshots = ExtractTradeSnapshots(rawJson, entries);
        return entries
            .Select((entry, idx) => new BittradeExecutionNormalized(
                entry.Id.ToString(),
                entry.Direction,
                entry.Price,
                entry.Amount,
                entry.Ts,
                snapshots[idx],
                new Dictionary<string, JsonElement>()))
            .ToList();
    }

    internal static IReadOnlyList<BittradeSymbolNormalized> NormalizeSymbols(IReadOnlyList<RawSymbolInfo> symbols)
    {
        if (symbols is null) throw new ArgumentNullException(nameof(symbols));

        return symbols
            .Select(symbol => new BittradeSymbolNormalized(
                Symbol: symbol.Symbol,
                BaseCurrency: symbol.BaseCurrency,
                QuoteCurrency: symbol.QuoteCurrency,
                PricePrecision: symbol.PricePrecision,
                AmountPrecision: symbol.AmountPrecision,
                MinOrderAmount: ParseDecimalFlexible(symbol.MinOrderAmount, "min-order-amt"),
                MinOrderValue: ParseNullableDecimalFlexible(symbol.MinOrderValue, "min-order-value"),
                State: symbol.State))
            .ToList();
    }

    internal static IReadOnlyList<BittradeBalanceEntryNormalized> NormalizeBalances(RawBalanceData data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));

        return data.List
            .Select(entry => new BittradeBalanceEntryNormalized(
                entry.Currency,
                entry.Type,
                ParseDecimalOrThrow(entry.Balance, "balance", "RawBalanceEntry")))
            .ToList();
    }

    private static IReadOnlyList<BittradeOrderBookLevelNormalized> NormalizeLevels(
        IReadOnlyList<IReadOnlyList<decimal>>? levels,
        string field)
    {
        if (levels is null)
        {
            throw new BittradeNormalizedException($"Bittrade order book missing {field}.");
        }

        return levels.Select(level => NormalizeLevel(level, field)).ToList();
    }

    private static BittradeOrderBookLevelNormalized NormalizeLevel(IReadOnlyList<decimal> level, string field)
    {
        if (level.Count < 2)
        {
            throw new BittradeNormalizedException($"Bittrade order book {field} level is invalid.");
        }

        return new BittradeOrderBookLevelNormalized(level[0], level[1]);
    }

    private static decimal ParseDecimalFlexible(JsonElement element, string field)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => ParseDecimalOrThrow(element.GetString()!, field, "RawSymbolInfo"),
            JsonValueKind.Number => element.GetDecimal(),
            _ => throw new BittradeNormalizedException($"Unexpected JSON type for RawSymbolInfo.{field}: {element.ValueKind}")
        };
    }

    private static decimal? ParseNullableDecimalFlexible(JsonElement element, string field)
    {
        if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined) return null;
        if (element.ValueKind == JsonValueKind.String)
        {
            var value = element.GetString();
            return string.IsNullOrWhiteSpace(value) ? null : ParseDecimalOrThrow(value, field, "RawSymbolInfo");
        }

        return element.ValueKind == JsonValueKind.Number ? element.GetDecimal() : null;
    }

    private static decimal ParseDecimalOrThrow(string s, string field, string dto)
    {
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new BittradeNormalizedException($"Invalid decimal for {dto}.{field}: '{s}'.");
    }

    private static IReadOnlyList<JsonElement> ExtractTradeSnapshots(
        string? rawJson,
        IReadOnlyList<RawTradeEntry> entries)
    {
        if (entries.Count == 0)
        {
            return Array.Empty<JsonElement>();
        }

        if (!string.IsNullOrWhiteSpace(rawJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(rawJson);
                if (doc.RootElement.ValueKind == JsonValueKind.Object
                    && doc.RootElement.TryGetProperty("tick", out var tick)
                    && tick.ValueKind == JsonValueKind.Object
                    && tick.TryGetProperty("data", out var data)
                    && data.ValueKind == JsonValueKind.Array)
                {
                    var snapshots = new List<JsonElement>(entries.Count);
                    for (var i = 0; i < entries.Count; i++)
                    {
                        if (i < data.GetArrayLength())
                        {
                            snapshots.Add(data[i].Clone());
                        }
                        else
                        {
                            snapshots.Add(EmptySnapshot());
                        }
                    }

                    return snapshots;
                }
            }
            catch (JsonException)
            {
            }
        }

        return entries
            .Select(entry => ExtractSnapshot(Serialize(entry)))
            .ToArray();
    }

    private static JsonElement ExtractSnapshot(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return EmptySnapshot();
        }

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            return doc.RootElement.Clone();
        }
        catch (JsonException)
        {
            return EmptySnapshot();
        }
    }

    private static JsonElement EmptySnapshot()
    {
        using var doc = JsonDocument.Parse("{}");
        return doc.RootElement.Clone();
    }

    private static string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, SerializerOptions);
}
