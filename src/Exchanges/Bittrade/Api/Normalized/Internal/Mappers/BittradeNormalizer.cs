using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using RawPublicDtos = ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;
using RawPublicRequests = ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;

internal static class BittradeNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    internal static bool TryNormalizeTicker(
        RawPublicDtos.RawMergedResponse response,
        string? rawJson,
        out BittradeTickerNormalized? normalized,
        out CallError? error)
    {
        if (response is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade ticker response invalid.");
            return false;
        }

        if (response.Tick is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade ticker response missing tick.");
            return false;
        }

        var tick = response.Tick;
        var timestamp = tick.Ts ?? response.Ts ?? DateTimeOffset.UtcNow;
        var snapshot = ExtractSnapshot(rawJson ?? Serialize(response));
        normalized = new BittradeTickerNormalized(
            tick.Close,
            timestamp,
            snapshot,
            new Dictionary<FreeText, JsonElement>());
        error = null;
        return true;
    }


    internal static bool TryNormalizeExecutions(
        IReadOnlyList<RawPublicDtos.RawTradeEntry> entries,
        string? rawJson,
        out IReadOnlyList<BittradeExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade trades response missing data.");
            return false;
        }

        var snapshots = ExtractTradeSnapshots(rawJson, entries);
        var mapped = new List<BittradeExecutionNormalized>(entries.Count);
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (!TryMapSide(entry.Direction, out var side, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BittradeExecutionNormalized(
                new OrderId(entry.Id.ToString()),
                side,
                entry.Price,
                entry.Amount,
                entry.Ts,
                snapshots[i],
                new Dictionary<FreeText, JsonElement>()));
        }

        normalized = mapped;
        error = null;
        return true;
    }

    internal static bool TryNormalizeTradeHistory(
        IReadOnlyList<RawPublicDtos.RawTradeHistoryEntry>? entries,
        out IReadOnlyList<BittradeExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<BittradeExecutionNormalized>();
            error = null;
            return true;
        }

        var flattened = entries
            .SelectMany(entry => entry.Data ?? Array.Empty<RawPublicDtos.RawTradeEntry>())
            .ToList();

        return TryNormalizeExecutions(flattened, rawJson: null, out normalized, out error);
    }

    internal static bool TryNormalizeSymbols(
        IReadOnlyList<RawPublicDtos.RawSymbolInfo> symbols,
        out IReadOnlyList<BittradeSymbolNormalized>? normalized,
        out CallError? error)
    {
        if (symbols is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade symbols response invalid.");
            return false;
        }

        var mapped = new List<BittradeSymbolNormalized>(symbols.Count);
        foreach (var symbol in symbols)
        {
            if (!TryParseDecimalFlexible(symbol.MinOrderAmount, "min-order-amt", out var minOrderAmount, out error))
            {
                normalized = null;
                return false;
            }

            if (!TryParseNullableDecimalFlexible(symbol.MinOrderValue, "min-order-value", out var minOrderValue, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BittradeSymbolNormalized(
                Symbol: Symbol.Parse(symbol.Symbol),
                BaseCurrency: FreeText.Parse(symbol.BaseCurrency),
                QuoteCurrency: FreeText.Parse(symbol.QuoteCurrency),
                PricePrecision: symbol.PricePrecision,
                AmountPrecision: symbol.AmountPrecision,
                MinOrderAmount: minOrderAmount,
                MinOrderValue: minOrderValue,
                State: FreeText.Parse(symbol.State)));
        }

        normalized = mapped;
        error = null;
        return true;
    }

    internal static bool TryNormalizeKlines(
        IReadOnlyList<RawPublicDtos.RawKlineEntry>? entries,
        out IReadOnlyList<BittradeKlineNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (entries is null || entries.Count == 0)
            {
                normalized = Array.Empty<BittradeKlineNormalized>();
                error = null;
                return true;
            }

            normalized = entries
                .Select(entry => new BittradeKlineNormalized(
                    Id: FreeText.Parse(entry.Id),
                    Open: entry.Open,
                    Close: entry.Close,
                    Low: entry.Low,
                    High: entry.High,
                    Amount: entry.Amount,
                    Volume: entry.Volume,
                    Count: entry.Count))
                .ToList();
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade klines response invalid.", ex);
            return false;
        }
    }

    internal static bool TryNormalizeTickers(
        IReadOnlyList<RawPublicDtos.RawTickerEntry>? entries,
        out IReadOnlyList<BittradeTickerEntryNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            normalized = BuildTickers(entries);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade tickers response invalid.", ex);
            return false;
        }
    }

    private static IReadOnlyList<BittradeTickerEntryNormalized> BuildTickers(
        IReadOnlyList<RawPublicDtos.RawTickerEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeTickerEntryNormalized>();
        }

        var now = DateTimeOffset.UtcNow;
        return entries
            .Select(entry => new BittradeTickerEntryNormalized(
                Symbol.Parse(entry.Symbol),
                entry.Close,
                now,
                ExtractSnapshot(Serialize(entry)),
                new Dictionary<FreeText, JsonElement>()))
            .ToList();
    }

    internal static bool TryNormalizeBalances(
        RawPrivateDtos.RawBalanceData data,
        out IReadOnlyList<BittradeBalanceEntryNormalized>? normalized,
        out CallError? error)
    {
        if (data is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade balance response invalid.");
            return false;
        }

        var mapped = new List<BittradeBalanceEntryNormalized>(data.List.Count);
        foreach (var entry in data.List)
        {
            if (!TryParseDecimal(entry.Balance, "balance", "RawBalanceEntry", out var balance, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BittradeBalanceEntryNormalized(
                FreeText.Parse(entry.Currency),
                FreeText.Parse(entry.Type),
                balance));
        }

        normalized = mapped;
        error = null;
        return true;
    }

    internal static bool TryNormalizeAccounts(
        IReadOnlyList<RawPrivateDtos.RawAccount>? accounts,
        out IReadOnlyList<BittradeAccountNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (accounts is null || accounts.Count == 0)
            {
                normalized = Array.Empty<BittradeAccountNormalized>();
                error = null;
                return true;
            }

            normalized = accounts
                .Select(account => new BittradeAccountNormalized(
                    FreeText.Parse(account.Id),
                    FreeText.Parse(account.Type),
                    ParseOptional(account.SubType),
                    FreeText.Parse(account.State)))
                .ToList();
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade accounts response invalid.", ex);
            return false;
        }
    }

    internal static bool TryNormalizeDepositWithdraws(
        IReadOnlyList<RawPrivateDtos.RawDepositWithdrawEntry>? entries,
        out IReadOnlyList<BittradeDepositWithdrawNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (entries is null || entries.Count == 0)
            {
                normalized = Array.Empty<BittradeDepositWithdrawNormalized>();
                error = null;
                return true;
            }

            normalized = entries
                .Select(entry => new BittradeDepositWithdrawNormalized(
                    FreeText.Parse(entry.Id),
                    FreeText.Parse(entry.Type),
                    FreeText.Parse(entry.Currency),
                    entry.Amount,
                    ParseOptional(entry.Address),
                    ParseOptional(entry.TxHash),
                    ParseOptional(entry.State),
                    entry.CreatedAt))
                .ToList();
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade deposit/withdraw response invalid.", ex);
            return false;
        }
    }

    internal static bool TryNormalizeWithdrawVirtualAddresses(
        IReadOnlyList<RawPrivateDtos.RawWithdrawVirtualAddress>? entries,
        out IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (entries is null || entries.Count == 0)
            {
                normalized = Array.Empty<BittradeWithdrawVirtualAddressNormalized>();
                error = null;
                return true;
            }

            normalized = entries
                .Select(entry => new BittradeWithdrawVirtualAddressNormalized(
                    AddressId: entry.AddressId,
                    Currency: ParseOptional(entry.Currency),
                    Address: ParseOptional(entry.Address),
                    AddressTag: ParseOptional(entry.AddressTag),
                    Chain: ParseOptional(entry.Chain),
                    Note: ParseOptional(entry.Note),
                    State: ParseOptional(entry.State),
                    CreatedAt: entry.CreatedAt,
                    UpdatedAt: entry.UpdatedAt))
                .ToList();
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade withdraw addresses response invalid.", ex);
            return false;
        }
    }

    internal static bool TryNormalizeRetailBalances(
        IReadOnlyList<RawPrivateDtos.RawRetailAccountBalanceEntry>? entries,
        out IReadOnlyList<BittradeRetailBalanceEntryNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<BittradeRetailBalanceEntryNormalized>();
            error = null;
            return true;
        }

        var mapped = new List<BittradeRetailBalanceEntryNormalized>(entries.Count);
        foreach (var entry in entries)
        {
            if (!TryParseNullableDecimal(entry.Balance, "balance", "RawRetailAccountBalanceEntry", out var balance, out error)
                || !TryParseNullableDecimal(entry.Available, "available", "RawRetailAccountBalanceEntry", out var available, out error)
                || !TryParseNullableDecimal(entry.Frozen, "frozen", "RawRetailAccountBalanceEntry", out var frozen, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BittradeRetailBalanceEntryNormalized(
                Currency: FreeText.Parse(entry.Currency),
                Balance: balance,
                Available: available,
                Frozen: frozen));
        }

        normalized = mapped;
        error = null;
        return true;
    }

    internal static bool TryNormalizeOrderBook(
        RawPublicDtos.RawDepthTick tick,
        out BittradeOrderBookNormalized? normalized,
        out CallError? error)
    {
        if (tick is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade order book response missing tick.");
            return false;
        }

        if (!TryNormalizeLevels(tick.Bids, "bids", out var bids, out error))
        {
            normalized = null;
            return false;
        }

        if (!TryNormalizeLevels(tick.Asks, "asks", out var asks, out error))
        {
            normalized = null;
            return false;
        }

        normalized = new BittradeOrderBookNormalized(bids, asks);
        error = null;
        return true;
    }

    private static bool TryNormalizeLevels(
        IReadOnlyList<IReadOnlyList<decimal>>? levels,
        string field,
        out IReadOnlyList<BittradeOrderBookLevelNormalized> normalized,
        out CallError? error)
    {
        if (levels is null)
        {
            normalized = Array.Empty<BittradeOrderBookLevelNormalized>();
            error = new CallError(CallErrorKind.Mapping, $"Bittrade order book missing {field}.");
            return false;
        }

        var mapped = new List<BittradeOrderBookLevelNormalized>(levels.Count);
        foreach (var level in levels)
        {
            if (level.Count < 2)
            {
                normalized = Array.Empty<BittradeOrderBookLevelNormalized>();
                error = new CallError(CallErrorKind.Mapping, $"Bittrade order book {field} level is invalid.");
                return false;
            }

            mapped.Add(new BittradeOrderBookLevelNormalized(level[0], level[1]));
        }

        normalized = mapped;
        error = null;
        return true;
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

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;

    private static bool TryParseDecimalFlexible(
        string? value,
        string field,
        out decimal parsed,
        out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            parsed = default;
            error = new CallError(CallErrorKind.Mapping, $"RawSymbolInfo.{field} is empty.");
            return false;
        }

        return TryParseDecimal(value, field, "RawSymbolInfo", out parsed, out error);
    }

    private static bool TryParseNullableDecimalFlexible(
        string? value,
        string field,
        out decimal? parsed,
        out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            parsed = null;
            error = null;
            return true;
        }

        if (!TryParseDecimal(value, field, "RawSymbolInfo", out var valueParsed, out error))
        {
            parsed = null;
            return false;
        }

        parsed = valueParsed;
        return true;
    }

    private static bool TryParseDecimal(
        string s,
        string field,
        string dto,
        out decimal parsed,
        out CallError? error)
    {
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            parsed = value;
            error = null;
            return true;
        }

        parsed = default;
        error = new CallError(CallErrorKind.Mapping, $"Invalid decimal for {dto}.{field}: '{s}'.");
        return false;
    }

    private static bool TryParseNullableDecimal(
        string? value,
        string field,
        string dto,
        out decimal? parsed,
        out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            parsed = null;
            error = null;
            return true;
        }

        if (!TryParseDecimal(value, field, dto, out var valueParsed, out error))
        {
            parsed = null;
            return false;
        }

        parsed = valueParsed;
        return true;
    }

    private static bool TryMapSide(string? direction, out BittradeOrderSide side, out CallError? error)
    {
        if (!BittradeOrderSideParser.TryParse(direction, out side))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unsupported trade side: {direction ?? "<null>"}.");
            return false;
        }

        error = null;
        return true;
    }
}
