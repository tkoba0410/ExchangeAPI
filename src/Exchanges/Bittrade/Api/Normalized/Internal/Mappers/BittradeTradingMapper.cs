using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.CallCommon;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;

internal static class BittradeTradingMapper
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static bool TryToRaw(
        AccountId accountId,
        Symbol apiSymbol,
        BittradeOrderRequest request,
        out RawPrivateRequests.RawCreateOrderRequest? raw,
        out CallError? error)
    {
        if (accountId.IsEmpty)
        {
            raw = null;
            error = new CallError(CallErrorKind.Mapping, "accountId is required.");
            return false;
        }

        if (apiSymbol.IsEmpty)
        {
            raw = null;
            error = new CallError(CallErrorKind.Mapping, "apiSymbol is required.");
            return false;
        }

        if (!TryMapOrderType(request.Side, request.OrderType, out var mappedType, out error))
        {
            raw = null;
            return false;
        }

        if (!TryToRawOrderType(mappedType, out var rawType, out error))
        {
            raw = null;
            return false;
        }

        var price = request.Price?.Value;
        var size = FormatDecimal(request.Size.Value);

        raw = new RawPrivateRequests.RawCreateOrderRequest(
            AccountId: accountId,
            Symbol: apiSymbol,
            Type: new FreeText(rawType),
            Amount: new FreeText(size),
            Price: price is null ? null : new FreeText(FormatDecimal(price.Value)),
            Source: null);
        error = null;
        return true;
    }

    public static RawPrivateRequests.RawCreateOrderRequest ToRaw(AccountId accountId, Symbol apiSymbol, BittradeOrderRequest request)
    {
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        if (apiSymbol.IsEmpty)
        {
            throw new ArgumentException("apiSymbol is required.", nameof(apiSymbol));
        }

        var type = ToRawOrderType(MapOrderType(request.Side, request.OrderType));
        var price = request.Price?.Value;
        var size = FormatDecimal(request.Size.Value);

        return new RawPrivateRequests.RawCreateOrderRequest(
            AccountId: accountId,
            Symbol: apiSymbol,
            Type: new FreeText(type),
            Amount: new FreeText(size),
            Price: price is null ? null : new FreeText(FormatDecimal(price.Value)),
            Source: null);
    }

    public static BittradeOrderResult ToOrderResult(RawPrivateDtos.RawPlaceOrderResponse raw)
    {
        var orderId = raw.OrderId;
        var key = new OrderKey(OrderIdKind.ExchangeOrderId, orderId);
        var exchangeOrderId = string.IsNullOrWhiteSpace(orderId) ? (ExchangeOrderId?)null : new ExchangeOrderId(orderId);
        return new BittradeOrderResult(key, ExchangeOrderId: exchangeOrderId);
    }

    public static bool TryToOpenOrders(
        Symbol symbol,
        RawPrivateDtos.RawOpenOrdersResponse raw,
        out IReadOnlyList<BittradeOpenOrder>? orders,
        out CallError? error)
    {
        if (raw.Data is null || raw.Data.Count == 0)
        {
            orders = Array.Empty<BittradeOpenOrder>();
            error = null;
            return true;
        }

        var mapped = new List<BittradeOpenOrder>(raw.Data.Count);
        foreach (var order in raw.Data)
        {
            if (!TryToOpenOrder(symbol, order, out var mappedOrder, out error))
            {
                orders = null;
                return false;
            }

            mapped.Add(mappedOrder!);
        }

        orders = mapped;
        error = null;
        return true;
    }


    private static bool TryToOpenOrder(
        Symbol symbol,
        RawPrivateDtos.RawOrderSummary raw,
        out BittradeOpenOrder? order,
        out CallError? error)
    {
        if (!TryMapSideAndType(raw.Type, out var side, out var type, out error))
        {
            order = null;
            return false;
        }

        if (!TryMapStatus(raw.State, out var status, out error))
        {
            order = null;
            return false;
        }

        if (!TryParseRequiredDecimal(raw.Amount, "amount", out var sizeValue, out error))
        {
            order = null;
            return false;
        }

        var size = new Size(sizeValue);
        if (!TryParseDecimal(raw.FilledAmount, "field-amount", out var executedValue, out error))
        {
            order = null;
            return false;
        }

        var executed = new Size(executedValue ?? 0m);
        var outstanding = new Size(Math.Max(0m, size.Value - executed.Value));

        if (!TryParseDecimal(raw.Price, "price", out var priceValue, out error))
        {
            order = null;
            return false;
        }

        var price = priceValue is null ? (Price?)null : new Price(priceValue.Value);

        order = new BittradeOpenOrder(
            Symbol: symbol,
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, raw.Id),
            Side: side,
            OrderType: type,
            Size: size,
            OutstandingSize: outstanding,
            ExecutedSize: executed,
            Price: price,
            OrderedAt: raw.CreatedAt,
            UpdatedAt: null,
            StopPrice: null,
            Status: ParseOptional(raw.State),
            ExchangeOrderId: new ExchangeOrderId(raw.Id));
        error = null;
        return true;
    }


    public static bool TryToOrderStatus(
        ProductCode productCode,
        RawPrivateDtos.RawOrderDetailResponse raw,
        OrderKey key,
        out BittradeOrderStatus? status,
        out CallError? error)
    {
        if (productCode.IsEmpty)
        {
            status = null;
            error = new CallError(CallErrorKind.Mapping, "productCode is required.");
            return false;
        }

        if (raw.Data is null)
        {
            status = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade order response is missing data.");
            return false;
        }

        if (!TryMapStatus(raw.Data.State, out var mappedStatus, out error))
        {
            status = null;
            return false;
        }

        if (!TryParseDecimal(raw.Data.Price, "price", out var priceValue, out error))
        {
            status = null;
            return false;
        }

        var price = priceValue is null ? (Price?)null : new Price(priceValue.Value);

        if (!TryParseRequiredDecimal(raw.Data.Amount, "amount", out var sizeValue, out error))
        {
            status = null;
            return false;
        }

        if (!TryParseDecimal(raw.Data.FilledAmount, "field-amount", out var executedValue, out error))
        {
            status = null;
            return false;
        }

        var executed = new Size(executedValue ?? 0m);
        var outstanding = new Size(Math.Max(0m, sizeValue - executed.Value));

        status = new BittradeOrderStatus(
            productCode,
            key,
            mappedStatus,
            executed,
            outstanding,
            price,
            null);
        error = null;
        return true;
    }


    public static bool TryToExecutions(
        IReadOnlyList<RawPrivateDtos.RawMatchResultEntry> entries,
        out IReadOnlyList<BittradeExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<BittradeExecutionNormalized>();
            error = null;
            return true;
        }

        var snapshots = entries
            .Select(entry => ExtractSnapshot(Serialize(entry)))
            .ToArray();

        var mapped = new List<BittradeExecutionNormalized>(entries.Count);
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (!TryMapExecutionSide(entry.Type, out var side, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BittradeExecutionNormalized(
                Id: new OrderId(string.IsNullOrWhiteSpace(entry.MatchId) ? entry.Id : entry.MatchId),
                Side: side,
                Price: entry.Price,
                Size: entry.FilledAmount,
                Timestamp: entry.CreatedAt,
                RawSnapshot: snapshots[i],
                Extras: new Dictionary<string, JsonElement>()));
        }

        normalized = mapped;
        error = null;
        return true;
    }


    public static bool TryToOrderSummaries(
        IReadOnlyList<RawPrivateDtos.RawOrderSummary>? entries,
        out IReadOnlyList<BittradeOrderSummaryNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<BittradeOrderSummaryNormalized>();
            error = null;
            return true;
        }

        var mapped = new List<BittradeOrderSummaryNormalized>(entries.Count);
        foreach (var entry in entries)
        {
            if (!TryParseRequiredDecimal(entry.Amount, "amount", out var amount, out error))
            {
                normalized = null;
                return false;
            }

            if (!TryParseDecimal(entry.Price, "price", out var price, out error))
            {
                normalized = null;
                return false;
            }

            if (!TryParseRequiredDecimal(entry.FilledAmount, "field-amount", out var filled, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BittradeOrderSummaryNormalized(
                Id: new OrderId(entry.Id),
                Symbol: Symbol.Parse(entry.Symbol),
                AccountId: FreeText.Parse(entry.AccountId),
                Amount: amount,
                Price: price,
                State: FreeText.Parse(entry.State),
                Type: FreeText.Parse(entry.Type),
                ClientOrderId: ParseOptional(entry.ClientOrderId),
                CreatedAt: entry.CreatedAt,
                FilledAmount: filled));
        }

        normalized = mapped;
        error = null;
        return true;
    }


    public static bool TryToRetailOrders(
        IReadOnlyList<RawPrivateDtos.RawRetailOrderEntry>? entries,
        out IReadOnlyList<BittradeRetailOrderEntryNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<BittradeRetailOrderEntryNormalized>();
            error = null;
            return true;
        }

        var mapped = new List<BittradeRetailOrderEntryNormalized>(entries.Count);
        foreach (var entry in entries)
        {
            if (!TryParseDecimal(entry.Price, "price", out var price, out error)
                || !TryParseDecimal(entry.Amount, "amount", out var amount, out error)
                || !TryParseDecimal(entry.CashAmount, "cash_amount", out var cashAmount, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new BittradeRetailOrderEntryNormalized(
                Id: new OrderId(entry.Id),
                Symbol: Symbol.Parse(entry.Symbol),
                Type: entry.Type,
                Price: price,
                Amount: amount,
                CashAmount: cashAmount,
                Status: entry.Status,
                CreatedAt: entry.CreatedAt));
        }

        normalized = mapped;
        error = null;
        return true;
    }


    public static bool TryToRetailOrder(
        RawPrivateDtos.RawRetailOrderEntry? entry,
        out BittradeRetailOrderEntryNormalized? normalized,
        out CallError? error)
    {
        if (entry is null)
        {
            normalized = null;
            error = null;
            return true;
        }

        if (!TryParseDecimal(entry.Price, "price", out var price, out error)
            || !TryParseDecimal(entry.Amount, "amount", out var amount, out error)
            || !TryParseDecimal(entry.CashAmount, "cash_amount", out var cashAmount, out error))
        {
            normalized = null;
            return false;
        }

        normalized = new BittradeRetailOrderEntryNormalized(
            Id: new OrderId(entry.Id),
            Symbol: Symbol.Parse(entry.Symbol),
            Type: entry.Type,
            Price: price,
            Amount: amount,
            CashAmount: cashAmount,
            Status: entry.Status,
            CreatedAt: entry.CreatedAt);
        error = null;
        return true;
    }


    public static BittradeRetailOrderResult ToRetailOrderResult(RawPrivateDtos.RawRetailOrderResponse raw)
    {
        return new BittradeRetailOrderResult(
            Code: raw.Code,
            OrderId: raw.Data,
            Success: raw.Success,
            Message: ParseOptional(raw.Message));
    }

    public static BittradeWithdrawResult ToWithdrawResult(RawPrivateDtos.RawCreateWithdrawResponse raw)
    {
        return new BittradeWithdrawResult(FreeText.Parse(raw.Status), raw.Data);
    }

    public static BittradeWithdrawResult ToWithdrawResult(RawPrivateDtos.RawCancelWithdrawResponse raw)
    {
        return new BittradeWithdrawResult(FreeText.Parse(raw.Status), raw.Data);
    }

    public static bool TryToRawRetailOrder(
        BittradeRetailOrderRequest request,
        out RawPrivateRequests.RawCreateRetailOrderRequest? raw,
        out CallError? error)
    {
        if (request is null)
        {
            raw = null;
            error = new CallError(CallErrorKind.Mapping, "request is required.");
            return false;
        }

        if (!BittradeSymbol.TryParse(request.Symbol.Value, out var symbol))
        {
            raw = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade symbol is invalid.");
            return false;
        }

        raw = new RawPrivateRequests.RawCreateRetailOrderRequest(
            Symbol: new Symbol(symbol.Value),
            Type: request.Type,
            Price: request.Price is null ? null : new FreeText(FormatDecimal(request.Price.Value)),
            Amount: request.Amount is null ? null : new FreeText(FormatDecimal(request.Amount.Value)),
            CashAmount: request.CashAmount is null ? null : new FreeText(FormatDecimal(request.CashAmount.Value)));
        error = null;
        return true;
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

    private static BittradeOrderSide MapSide(string? side)
    {
        return BittradeOrderSideParser.ParseOrThrow(side, "execution");
    }

    private static bool TryMapExecutionSide(string? side, out BittradeOrderSide parsed, out CallError? error)
    {
        if (!BittradeOrderSideParser.TryParse(side, out parsed))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unsupported execution side: {side ?? "<null>"}.");
            return false;
        }

        error = null;
        return true;
    }

    private static BittradeOrderType MapOrderType(Side side, OrderType type)
    {
        return (side, type) switch
        {
            (Side.Buy, OrderType.Market) => BittradeOrderType.BuyMarket,
            (Side.Sell, OrderType.Market) => BittradeOrderType.SellMarket,
            (Side.Buy, OrderType.Limit) => BittradeOrderType.BuyLimit,
            (Side.Sell, OrderType.Limit) => BittradeOrderType.SellLimit,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported order type.")
        };
    }

    private static bool TryMapOrderType(Side side, OrderType type, out BittradeOrderType mapped, out CallError? error)
    {
        switch (side, type)
        {
            case (Side.Buy, OrderType.Market):
                mapped = BittradeOrderType.BuyMarket;
                error = null;
                return true;
            case (Side.Sell, OrderType.Market):
                mapped = BittradeOrderType.SellMarket;
                error = null;
                return true;
            case (Side.Buy, OrderType.Limit):
                mapped = BittradeOrderType.BuyLimit;
                error = null;
                return true;
            case (Side.Sell, OrderType.Limit):
                mapped = BittradeOrderType.SellLimit;
                error = null;
                return true;
            default:
                mapped = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order type: {type}.");
                return false;
        }
    }

    private static (Side Side, OrderType OrderType) MapSideAndType(string type)
    {
        var parsedType = ParseOrderType(type);
        var parsedSide = parsedType switch
        {
            BittradeOrderType.BuyLimit or BittradeOrderType.BuyMarket or BittradeOrderType.BuyLimitMaker or BittradeOrderType.BuyIoc => Side.Buy,
            BittradeOrderType.SellLimit or BittradeOrderType.SellMarket or BittradeOrderType.SellLimitMaker or BittradeOrderType.SellIoc => Side.Sell,
            _ => throw new InvalidOperationException($"Unsupported order side: {type}.")
        };

        var orderType = parsedType switch
        {
            BittradeOrderType.BuyMarket or BittradeOrderType.SellMarket => OrderType.Market,
            BittradeOrderType.BuyLimit or BittradeOrderType.SellLimit => OrderType.Limit,
            _ => throw new InvalidOperationException($"Unsupported order type: {type}.")
        };

        return (parsedSide, orderType);
    }

    private static bool TryMapSideAndType(
        string type,
        out Side side,
        out OrderType orderType,
        out CallError? error)
    {
        if (!TryParseOrderType(type, out var parsedType, out error))
        {
            side = default;
            orderType = default;
            return false;
        }

        switch (parsedType)
        {
            case BittradeOrderType.BuyLimit:
            case BittradeOrderType.BuyMarket:
            case BittradeOrderType.BuyLimitMaker:
            case BittradeOrderType.BuyIoc:
                side = Side.Buy;
                break;
            case BittradeOrderType.SellLimit:
            case BittradeOrderType.SellMarket:
            case BittradeOrderType.SellLimitMaker:
            case BittradeOrderType.SellIoc:
                side = Side.Sell;
                break;
            default:
                side = default;
                orderType = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order side: {type}.");
                return false;
        }

        switch (parsedType)
        {
            case BittradeOrderType.BuyMarket:
            case BittradeOrderType.SellMarket:
                orderType = OrderType.Market;
                break;
            case BittradeOrderType.BuyLimit:
            case BittradeOrderType.SellLimit:
                orderType = OrderType.Limit;
                break;
            default:
                orderType = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order type: {type}.");
                return false;
        }

        error = null;
        return true;
    }

    private static ExchangeApi.Primitives.DomainCommon.Enums.OrderState MapStatus(string state)
    {
        return ParseOrderState(state) switch
        {
            BittradeOrderState.Submitted => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Active,
            BittradeOrderState.PartialFilled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Active,
            BittradeOrderState.Filled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Completed,
            BittradeOrderState.PartialCanceled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Canceled,
            BittradeOrderState.Canceled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Canceled,
            _ => throw new InvalidOperationException($"Unsupported order state: {state}.")
        };
    }

    private static bool TryMapStatus(
        string state,
        out ExchangeApi.Primitives.DomainCommon.Enums.OrderState mapped,
        out CallError? error)
    {
        if (!TryParseOrderState(state, out var parsed, out error))
        {
            mapped = default;
            return false;
        }

        mapped = parsed switch
        {
            BittradeOrderState.Submitted => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Active,
            BittradeOrderState.PartialFilled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Active,
            BittradeOrderState.Filled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Completed,
            BittradeOrderState.PartialCanceled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Canceled,
            BittradeOrderState.Canceled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Canceled,
            _ => default
        };

        if (mapped == default)
        {
            error = new CallError(CallErrorKind.Mapping, $"Unsupported order state: {state}.");
            return false;
        }

        error = null;
        return true;
    }

    private static string ToRawOrderType(BittradeOrderType type) =>
        type switch
        {
            BittradeOrderType.BuyLimit => "buy-limit",
            BittradeOrderType.SellLimit => "sell-limit",
            BittradeOrderType.BuyMarket => "buy-market",
            BittradeOrderType.SellMarket => "sell-market",
            BittradeOrderType.BuyLimitMaker => "buy-limit-maker",
            BittradeOrderType.SellLimitMaker => "sell-limit-maker",
            BittradeOrderType.BuyIoc => "buy-ioc",
            BittradeOrderType.SellIoc => "sell-ioc",
            _ => throw new InvalidOperationException($"Unsupported order type: {type}.")
        };

    private static bool TryToRawOrderType(BittradeOrderType type, out string raw, out CallError? error)
    {
        raw = type switch
        {
            BittradeOrderType.BuyLimit => "buy-limit",
            BittradeOrderType.SellLimit => "sell-limit",
            BittradeOrderType.BuyMarket => "buy-market",
            BittradeOrderType.SellMarket => "sell-market",
            BittradeOrderType.BuyLimitMaker => "buy-limit-maker",
            BittradeOrderType.SellLimitMaker => "sell-limit-maker",
            BittradeOrderType.BuyIoc => "buy-ioc",
            BittradeOrderType.SellIoc => "sell-ioc",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(raw))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unsupported order type: {type}.");
            return false;
        }

        error = null;
        return true;
    }

    private static BittradeOrderType ParseOrderType(string type) =>
        type switch
        {
            "buy-limit" => BittradeOrderType.BuyLimit,
            "sell-limit" => BittradeOrderType.SellLimit,
            "buy-market" => BittradeOrderType.BuyMarket,
            "sell-market" => BittradeOrderType.SellMarket,
            "buy-limit-maker" => BittradeOrderType.BuyLimitMaker,
            "sell-limit-maker" => BittradeOrderType.SellLimitMaker,
            "buy-ioc" => BittradeOrderType.BuyIoc,
            "sell-ioc" => BittradeOrderType.SellIoc,
            _ => throw new InvalidOperationException($"Unsupported order type: {type}.")
        };

    private static bool TryParseOrderType(string type, out BittradeOrderType parsed, out CallError? error)
    {
        switch (type)
        {
            case "buy-limit":
                parsed = BittradeOrderType.BuyLimit;
                error = null;
                return true;
            case "sell-limit":
                parsed = BittradeOrderType.SellLimit;
                error = null;
                return true;
            case "buy-market":
                parsed = BittradeOrderType.BuyMarket;
                error = null;
                return true;
            case "sell-market":
                parsed = BittradeOrderType.SellMarket;
                error = null;
                return true;
            case "buy-limit-maker":
                parsed = BittradeOrderType.BuyLimitMaker;
                error = null;
                return true;
            case "sell-limit-maker":
                parsed = BittradeOrderType.SellLimitMaker;
                error = null;
                return true;
            case "buy-ioc":
                parsed = BittradeOrderType.BuyIoc;
                error = null;
                return true;
            case "sell-ioc":
                parsed = BittradeOrderType.SellIoc;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order type: {type}.");
                return false;
        }
    }

    private static BittradeOrderState ParseOrderState(string state) =>
        state switch
        {
            "submitted" => BittradeOrderState.Submitted,
            "partial-filled" => BittradeOrderState.PartialFilled,
            "filled" => BittradeOrderState.Filled,
            "partial-canceled" => BittradeOrderState.PartialCanceled,
            "canceled" => BittradeOrderState.Canceled,
            _ => throw new InvalidOperationException($"Unsupported order state: {state}.")
        };

    private static bool TryParseOrderState(string state, out BittradeOrderState parsed, out CallError? error)
    {
        switch (state)
        {
            case "submitted":
                parsed = BittradeOrderState.Submitted;
                error = null;
                return true;
            case "partial-filled":
                parsed = BittradeOrderState.PartialFilled;
                error = null;
                return true;
            case "filled":
                parsed = BittradeOrderState.Filled;
                error = null;
                return true;
            case "partial-canceled":
                parsed = BittradeOrderState.PartialCanceled;
                error = null;
                return true;
            case "canceled":
                parsed = BittradeOrderState.Canceled;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order state: {state}.");
                return false;
        }
    }
    private static string FormatDecimal(decimal value) =>
        value.ToString(CultureInfo.InvariantCulture);

    private static decimal? ParseDecimalOrThrow(string? text, string field)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Invalid {field}: '{text}'.");
    }

    private static decimal ParseRequiredDecimal(string? text, string field)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException($"Missing {field}: <missing>.");
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Invalid {field}: '{text}'.");
    }

    private static bool TryParseDecimal(string? text, string field, out decimal? parsed, out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            parsed = null;
            error = null;
            return true;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            parsed = value;
            error = null;
            return true;
        }

        parsed = null;
        error = new CallError(CallErrorKind.Mapping, $"Invalid {field}: '{text}'.");
        return false;
    }

    private static bool TryParseRequiredDecimal(string? text, string field, out decimal parsed, out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            parsed = default;
            error = new CallError(CallErrorKind.Mapping, $"Missing {field}: <missing>.");
            return false;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            parsed = value;
            error = null;
            return true;
        }

        parsed = default;
        error = new CallError(CallErrorKind.Mapping, $"Invalid {field}: '{text}'.");
        return false;
    }

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;
}
