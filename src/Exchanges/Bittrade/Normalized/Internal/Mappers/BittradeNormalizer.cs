using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Account;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using RawPublicDtos = ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using RawPublicRequests = ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;

internal static class BittradeNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    internal static BittradeTickerNormalized NormalizeTicker(RawPublicDtos.RawMergedResponse response, string? rawJson)
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

    internal static BittradeOrderBookNormalized NormalizeOrderBook(RawPublicDtos.RawDepthTick tick)
    {
        if (tick is null) throw new ArgumentNullException(nameof(tick));

        var bids = NormalizeLevels(tick.Bids, "bids");
        var asks = NormalizeLevels(tick.Asks, "asks");
        return new BittradeOrderBookNormalized(bids, asks);
    }

    internal static IReadOnlyList<BittradeExecutionNormalized> NormalizeExecutions(
        IReadOnlyList<RawPublicDtos.RawTradeEntry> entries,
        string? rawJson)
    {
        if (entries is null) throw new ArgumentNullException(nameof(entries));

        var snapshots = ExtractTradeSnapshots(rawJson, entries);
        return entries
            .Select((entry, idx) => new BittradeExecutionNormalized(
                entry.Id.ToString(),
                MapSide(entry.Direction),
                entry.Price,
                entry.Amount,
                entry.Ts,
                snapshots[idx],
                new Dictionary<string, JsonElement>()))
            .ToList();
    }

    internal static IReadOnlyList<BittradeExecutionNormalized> NormalizeTradeHistory(
        IReadOnlyList<RawPublicDtos.RawTradeHistoryEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeExecutionNormalized>();
        }

        var flattened = entries
            .SelectMany(entry => entry.Data ?? Array.Empty<RawPublicDtos.RawTradeEntry>())
            .ToList();
        return NormalizeExecutions(flattened, rawJson: null);
    }

    internal static IReadOnlyList<BittradeSymbolNormalized> NormalizeSymbols(IReadOnlyList<RawPublicDtos.RawSymbolInfo> symbols)
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

    internal static IReadOnlyList<BittradeKlineNormalized> NormalizeKlines(
        IReadOnlyList<RawPublicDtos.RawKlineEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeKlineNormalized>();
        }

        return entries
            .Select(entry => new BittradeKlineNormalized(
                Id: entry.Id,
                Open: entry.Open,
                Close: entry.Close,
                Low: entry.Low,
                High: entry.High,
                Amount: entry.Amount,
                Volume: entry.Volume,
                Count: entry.Count))
            .ToList();
    }

    internal static IReadOnlyList<BittradeTickerEntryNormalized> NormalizeTickers(
        IReadOnlyList<RawPublicDtos.RawTickerEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeTickerEntryNormalized>();
        }

        var now = DateTimeOffset.UtcNow;
        return entries
            .Select(entry => new BittradeTickerEntryNormalized(
                entry.Symbol,
                entry.Close,
                now,
                ExtractSnapshot(Serialize(entry)),
                new Dictionary<string, JsonElement>()))
            .ToList();
    }

    internal static IReadOnlyList<BittradeBalanceEntryNormalized> NormalizeBalances(RawPrivateDtos.RawBalanceData data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));

        return data.List
            .Select(entry => new BittradeBalanceEntryNormalized(
                entry.Currency,
                entry.Type,
                ParseDecimalOrThrow(entry.Balance, "balance", "RawBalanceEntry")))
            .ToList();
    }

    internal static IReadOnlyList<BittradeAccountNormalized> NormalizeAccounts(
        IReadOnlyList<RawPrivateDtos.RawAccount>? accounts)
    {
        if (accounts is null || accounts.Count == 0)
        {
            return Array.Empty<BittradeAccountNormalized>();
        }

        return accounts
            .Select(account => new BittradeAccountNormalized(
                account.Id,
                account.Type,
                account.SubType,
                account.State))
            .ToList();
    }

    internal static IReadOnlyList<BittradeDepositWithdrawNormalized> NormalizeDepositWithdraws(
        IReadOnlyList<RawPrivateDtos.RawDepositWithdrawEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeDepositWithdrawNormalized>();
        }

        return entries
            .Select(entry => new BittradeDepositWithdrawNormalized(
                entry.Id,
                entry.Type,
                entry.Currency,
                entry.Amount,
                entry.Address,
                entry.TxHash,
                entry.State,
                entry.CreatedAt))
            .ToList();
    }

    internal static IReadOnlyList<BittradeWithdrawVirtualAddressNormalized> NormalizeWithdrawVirtualAddresses(
        IReadOnlyList<RawPrivateDtos.RawWithdrawVirtualAddress>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeWithdrawVirtualAddressNormalized>();
        }

        return entries
            .Select(entry => new BittradeWithdrawVirtualAddressNormalized(
                AddressId: entry.AddressId,
                Currency: entry.Currency,
                Address: entry.Address,
                AddressTag: entry.AddressTag,
                Chain: entry.Chain,
                Note: entry.Note,
                State: entry.State,
                CreatedAt: entry.CreatedAt,
                UpdatedAt: entry.UpdatedAt))
            .ToList();
    }

    internal static IReadOnlyList<BittradeRetailBalanceEntryNormalized> NormalizeRetailBalances(
        IReadOnlyList<RawPrivateDtos.RawRetailAccountBalanceEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeRetailBalanceEntryNormalized>();
        }

        return entries
            .Select(entry => new BittradeRetailBalanceEntryNormalized(
                Currency: entry.Currency ?? string.Empty,
                Balance: ParseNullableDecimal(entry.Balance, "balance", "RawRetailAccountBalanceEntry"),
                Available: ParseNullableDecimal(entry.Available, "available", "RawRetailAccountBalanceEntry"),
                Frozen: ParseNullableDecimal(entry.Frozen, "frozen", "RawRetailAccountBalanceEntry")))
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

    private static decimal ParseDecimalFlexible(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BittradeNormalizedException($"RawSymbolInfo.{field} is empty.");
        }

        return ParseDecimalOrThrow(value, field, "RawSymbolInfo");
    }

    private static decimal? ParseNullableDecimalFlexible(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ParseDecimalOrThrow(value, field, "RawSymbolInfo");
    }

    private static decimal ParseDecimalOrThrow(string s, string field, string dto)
    {
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new BittradeNormalizedException($"Invalid decimal for {dto}.{field}: '{s}'.");
    }

    private static decimal? ParseNullableDecimal(string? value, string field, string dto)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ParseDecimalOrThrow(value, field, dto);
    }

    private static IReadOnlyList<JsonElement> ExtractTradeSnapshots(
        string? rawJson,
        IReadOnlyList<RawPublicDtos.RawTradeEntry> entries)
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

    private static BittradeOrderSide MapSide(string? direction)
    {
        try
        {
            return BittradeOrderSideParser.ParseOrThrow(direction, "trade");
        }
        catch (ArgumentException ex)
        {
            throw new BittradeNormalizedException(ex.Message);
        }
    }
}
