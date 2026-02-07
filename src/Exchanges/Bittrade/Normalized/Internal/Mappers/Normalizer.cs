using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using RawPublicDtos = ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using RawPublicRequests = ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;

internal static class Normalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    internal static bool TryNormalizeTicker(
        RawPublicDtos.GetDetailMergedResponse response,
        string? rawJson,
        out TickerNormalized? normalized,
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
        normalized = new TickerNormalized(
            tick.Close,
            timestamp,
            snapshot,
            new Dictionary<FreeText, JsonElement>());
        error = null;
        return true;
    }


    internal static bool TryNormalizeExecutions(
        IReadOnlyList<RawPublicDtos.GetHistoryTradeEntry> entries,
        string? rawJson,
        out IReadOnlyList<ExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade trades response missing data.");
            return false;
        }

        var snapshots = ExtractTradeSnapshots(rawJson, entries);
        var mapped = new List<ExecutionNormalized>(entries.Count);
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (!TryMapSide(entry.Direction, out var side, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new ExecutionNormalized(
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
        out IReadOnlyList<ExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<ExecutionNormalized>();
            error = null;
            return true;
        }

        var flattened = entries
            .SelectMany(entry => entry.Data ?? Array.Empty<RawPublicDtos.GetHistoryTradeEntry>())
            .ToList();

        return TryNormalizeExecutions(flattened, rawJson: null, out normalized, out error);
    }

    internal static bool TryNormalizeSymbols(
        IReadOnlyList<RawPublicDtos.RawSymbolInfo> symbols,
        out IReadOnlyList<SymbolNormalized>? normalized,
        out CallError? error)
    {
        if (symbols is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade symbols response invalid.");
            return false;
        }

        var mapped = new List<SymbolNormalized>(symbols.Count);
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

            mapped.Add(new SymbolNormalized(
                Symbol: Symbol.Parse(symbol.Symbol),
                BaseCurrency: CurrencyCodeConverter.FromString(symbol.BaseCurrency),
                QuoteCurrency: CurrencyCodeConverter.FromString(symbol.QuoteCurrency),
                PricePrecision: symbol.PricePrecision,
                AmountPrecision: symbol.AmountPrecision,
                MinOrderAmount: minOrderAmount,
                MinOrderValue: minOrderValue,
                State: ParseSymbolState(symbol.State)));
        }

        normalized = mapped;
        error = null;
        return true;
    }

    internal static bool TryNormalizeKlines(
        IReadOnlyList<RawPublicDtos.RawKlineEntry>? entries,
        out IReadOnlyList<KlineNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (entries is null || entries.Count == 0)
            {
                normalized = Array.Empty<KlineNormalized>();
                error = null;
                return true;
            }

            normalized = entries
                .Select(entry => new KlineNormalized(
                    OpenTimeUnix: FreeText.Parse(entry.Id),
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
        out IReadOnlyList<TickerEntryNormalized>? normalized,
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

    private static IReadOnlyList<TickerEntryNormalized> BuildTickers(
        IReadOnlyList<RawPublicDtos.RawTickerEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<TickerEntryNormalized>();
        }

        var now = DateTimeOffset.UtcNow;
        return entries
            .Select(entry => new TickerEntryNormalized(
                Symbol.Parse(entry.Symbol),
                entry.Close,
                now,
                ExtractSnapshot(Serialize(entry)),
                new Dictionary<FreeText, JsonElement>()))
            .ToList();
    }

    internal static bool TryNormalizeBalances(
        RawPrivateDtos.GetAccountsBalanceByAccountIdData data,
        out IReadOnlyList<BalanceEntryNormalized>? normalized,
        out CallError? error)
    {
        if (data is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade balance response invalid.");
            return false;
        }

        var mapped = new List<BalanceEntryNormalized>(data.List.Count);
        foreach (var entry in data.List)
        {
            if (!TryParseDecimal(entry.Balance, "balance", "GetAccountsBalanceByAccountIdEntry", out var balance, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BalanceEntryNormalized(
                CurrencyCodeConverter.FromString(entry.Currency),
                ParseBalanceType(entry.Type),
                balance));
        }

        normalized = mapped;
        error = null;
        return true;
    }

    internal static bool TryNormalizeAccounts(
        IReadOnlyList<RawPrivateDtos.GetAccountsItem>? accounts,
        out IReadOnlyList<AccountNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (accounts is null || accounts.Count == 0)
            {
                normalized = Array.Empty<AccountNormalized>();
                error = null;
                return true;
            }

            normalized = accounts
                .Select(account => new AccountNormalized(
                    AccountId: AccountId.Parse(account.Id),
                    Type: ParseAccountType(account.Type),
                    SubType: ParseAccountSubType(account.SubType),
                    State: ParseAccountState(account.State)))
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
        IReadOnlyList<RawPrivateDtos.GetDepositWithdrawEntry>? entries,
        out IReadOnlyList<DepositWithdrawNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (entries is null || entries.Count == 0)
            {
                normalized = Array.Empty<DepositWithdrawNormalized>();
                error = null;
                return true;
            }

            normalized = entries
                .Select(entry => new DepositWithdrawNormalized(
                    TransactionId: TransactionId.Parse(entry.Id),
                    Type: ParseDepositWithdrawType(entry.Type),
                    Currency: CurrencyCodeConverter.FromString(entry.Currency),
                    entry.Amount,
                    ParseOptional(entry.Address),
                    ParseOptional(entry.TxHash),
                    ParseDepositWithdrawState(entry.State),
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
        IReadOnlyList<RawPrivateDtos.GetWithdrawVirtualAddressesItem>? entries,
        out IReadOnlyList<WithdrawVirtualAddressNormalized>? normalized,
        out CallError? error)
    {
        try
        {
            if (entries is null || entries.Count == 0)
            {
                normalized = Array.Empty<WithdrawVirtualAddressNormalized>();
                error = null;
                return true;
            }

            normalized = entries
                .Select(entry => new WithdrawVirtualAddressNormalized(
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
        IReadOnlyList<RawPrivateDtos.GetRetailAccountBalanceEntry>? entries,
        out IReadOnlyList<RetailBalanceEntryNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<RetailBalanceEntryNormalized>();
            error = null;
            return true;
        }

        var mapped = new List<RetailBalanceEntryNormalized>(entries.Count);
        foreach (var entry in entries)
        {
            if (!TryParseNullableDecimal(entry.Balance, "balance", "GetRetailAccountBalanceEntry", out var balance, out error)
                || !TryParseNullableDecimal(entry.Available, "available", "GetRetailAccountBalanceEntry", out var available, out error)
                || !TryParseNullableDecimal(entry.Frozen, "frozen", "GetRetailAccountBalanceEntry", out var frozen, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new RetailBalanceEntryNormalized(
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
        RawPublicDtos.GetDepthLevel tick,
        out OrderBookNormalized? normalized,
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

        normalized = new OrderBookNormalized(bids, asks);
        error = null;
        return true;
    }

    private static bool TryNormalizeLevels(
        IReadOnlyList<IReadOnlyList<decimal>>? levels,
        string field,
        out IReadOnlyList<OrderBookLevelNormalized> normalized,
        out CallError? error)
    {
        if (levels is null)
        {
            normalized = Array.Empty<OrderBookLevelNormalized>();
            error = new CallError(CallErrorKind.Mapping, $"Bittrade order book missing {field}.");
            return false;
        }

        var mapped = new List<OrderBookLevelNormalized>(levels.Count);
        foreach (var level in levels)
        {
            if (level.Count < 2)
            {
                normalized = Array.Empty<OrderBookLevelNormalized>();
                error = new CallError(CallErrorKind.Mapping, $"Bittrade order book {field} level is invalid.");
                return false;
            }

            mapped.Add(new OrderBookLevelNormalized(level[0], level[1]));
        }

        normalized = mapped;
        error = null;
        return true;
    }

    private static IReadOnlyList<JsonElement> ExtractTradeSnapshots(
        string? rawJson,
        IReadOnlyList<RawPublicDtos.GetHistoryTradeEntry> entries)
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

    private static Closed<ExchangeSymbolState> ParseSymbolState(string? state) =>
        (state ?? string.Empty).ToLowerInvariant() switch
        {
            "online" => Closed<ExchangeSymbolState>.KnownValue(ExchangeSymbolState.Online),
            "offline" => Closed<ExchangeSymbolState>.KnownValue(ExchangeSymbolState.Offline),
            _ => Closed<ExchangeSymbolState>.UnknownValue(state ?? string.Empty),
        };

    private static Closed<ExchangeBalanceType> ParseBalanceType(string? type) =>
        (type ?? string.Empty).ToLowerInvariant() switch
        {
            "trade" => Closed<ExchangeBalanceType>.KnownValue(ExchangeBalanceType.Trade),
            "frozen" => Closed<ExchangeBalanceType>.KnownValue(ExchangeBalanceType.Frozen),
            _ => Closed<ExchangeBalanceType>.UnknownValue(type ?? string.Empty),
        };

    private static Closed<ExchangeAccountType> ParseAccountType(string? type) =>
        (type ?? string.Empty).ToLowerInvariant() switch
        {
            "spot" => Closed<ExchangeAccountType>.KnownValue(ExchangeAccountType.Spot),
            _ => Closed<ExchangeAccountType>.UnknownValue(type ?? string.Empty),
        };

    private static Closed<ExchangeAccountSubType>? ParseAccountSubType(string? subType)
    {
        if (string.IsNullOrWhiteSpace(subType))
        {
            return null;
        }

        return subType.ToLowerInvariant() switch
        {
            "main" => Closed<ExchangeAccountSubType>.KnownValue(ExchangeAccountSubType.Main),
            _ => Closed<ExchangeAccountSubType>.UnknownValue(subType),
        };
    }

    private static Closed<ExchangeAccountState> ParseAccountState(string? state) =>
        (state ?? string.Empty).ToLowerInvariant() switch
        {
            "working" => Closed<ExchangeAccountState>.KnownValue(ExchangeAccountState.Working),
            _ => Closed<ExchangeAccountState>.UnknownValue(state ?? string.Empty),
        };

    private static Closed<ExchangeDepositWithdrawType> ParseDepositWithdrawType(string? type) =>
        (type ?? string.Empty).ToLowerInvariant() switch
        {
            "deposit" => Closed<ExchangeDepositWithdrawType>.KnownValue(ExchangeDepositWithdrawType.Deposit),
            "withdraw" => Closed<ExchangeDepositWithdrawType>.KnownValue(ExchangeDepositWithdrawType.Withdraw),
            _ => Closed<ExchangeDepositWithdrawType>.UnknownValue(type ?? string.Empty),
        };

    private static Closed<ExchangeDepositWithdrawState>? ParseDepositWithdrawState(string? state)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            return null;
        }

        return state.ToLowerInvariant() switch
        {
            "submitted" => Closed<ExchangeDepositWithdrawState>.KnownValue(ExchangeDepositWithdrawState.Submitted),
            "processing" => Closed<ExchangeDepositWithdrawState>.KnownValue(ExchangeDepositWithdrawState.Processing),
            "completed" => Closed<ExchangeDepositWithdrawState>.KnownValue(ExchangeDepositWithdrawState.Completed),
            "canceled" => Closed<ExchangeDepositWithdrawState>.KnownValue(ExchangeDepositWithdrawState.Canceled),
            "failed" => Closed<ExchangeDepositWithdrawState>.KnownValue(ExchangeDepositWithdrawState.Failed),
            _ => Closed<ExchangeDepositWithdrawState>.UnknownValue(state),
        };
    }

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

    private static bool TryMapSide(string? direction, out OrderSide side, out CallError? error)
    {
        if (!OrderSideParser.TryParse(direction, out side))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unsupported trade side: {direction ?? "<null>"}.");
            return false;
        }

        error = null;
        return true;
    }
}
