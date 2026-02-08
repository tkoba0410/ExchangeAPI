using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.CallCommon;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;

internal static class TradingMapper
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static bool TryToRaw(
        AccountId accountId,
        Symbol apiSymbol,
        OrderRequest request,
        out RawPrivateRequests.RawPostOrdersPlaceRequest? raw,
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

        raw = new RawPrivateRequests.RawPostOrdersPlaceRequest(
            AccountId: accountId,
            Symbol: apiSymbol,
            Type: new FreeText(rawType),
            Amount: new FreeText(size),
            Price: price is null ? null : new FreeText(FormatDecimal(price.Value)),
            Source: null);
        error = null;
        return true;
    }

    public static PostOrdersPlaceResponse ToPostOrdersPlaceResponse(RawPrivateDtos.PostOrdersPlaceResponse raw)
    {
        var orderId = raw.OrderId;
        var key = new OrderKey(OrderIdKind.ExchangeOrderId, orderId);
        var exchangeOrderId = string.IsNullOrWhiteSpace(orderId) ? (ExchangeOrderId?)null : new ExchangeOrderId(orderId);
        return new PostOrdersPlaceResponse(key, ExchangeOrderId: exchangeOrderId);
    }

    public static bool TryToOpenOrders(
        Symbol symbol,
        RawPrivateDtos.GetOpenOrdersResponse raw,
        out IReadOnlyList<OpenOrder>? orders,
        out CallError? error)
    {
        if (raw.Data is null || raw.Data.Count == 0)
        {
            orders = Array.Empty<OpenOrder>();
            error = null;
            return true;
        }

        var mapped = new List<OpenOrder>(raw.Data.Count);
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
        out OpenOrder? order,
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

        order = new OpenOrder(
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
        RawPrivateDtos.GetOrdersByOrderIdResponse raw,
        OrderKey key,
        out OrderStatus? status,
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

        status = new OrderStatus(
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
        out IReadOnlyList<ExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<ExecutionNormalized>();
            error = null;
            return true;
        }

        var snapshots = entries
            .Select(entry => ExtractSnapshot(Serialize(entry)))
            .ToArray();

        var mapped = new List<ExecutionNormalized>(entries.Count);
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (!TryMapExecutionSide(entry.Type, out var side, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new ExecutionNormalized(
                OrderId: new OrderId(string.IsNullOrWhiteSpace(entry.MatchId) ? entry.Id : entry.MatchId),
                Side: side,
                Price: entry.Price,
                Size: entry.FilledAmount,
                Timestamp: entry.CreatedAt,
                RawSnapshot: snapshots[i],
                Extras: new Dictionary<FreeText, JsonElement>()));
        }

        normalized = mapped;
        error = null;
        return true;
    }


    public static bool TryToOrderSummaries(
        IReadOnlyList<RawPrivateDtos.RawOrderSummary>? entries,
        out IReadOnlyList<OrderSummaryNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<OrderSummaryNormalized>();
            error = null;
            return true;
        }

        var mapped = new List<OrderSummaryNormalized>(entries.Count);
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

            mapped.Add(new OrderSummaryNormalized(
                OrderId: new OrderId(entry.Id),
                Symbol: Symbol.Parse(entry.Symbol),
                AccountId: AccountId.Parse(entry.AccountId),
                Amount: amount,
                Price: price,
                State: ParseOrderStateClosed(entry.State),
                Type: ParseOrderTypeClosed(entry.Type),
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
        out IReadOnlyList<RetailOrderEntryNormalized>? normalized,
        out CallError? error)
    {
        if (entries is null || entries.Count == 0)
        {
            normalized = Array.Empty<RetailOrderEntryNormalized>();
            error = null;
            return true;
        }

        var mapped = new List<RetailOrderEntryNormalized>(entries.Count);
        foreach (var entry in entries)
        {
            if (!TryParseDecimal(entry.Price, "price", out var price, out error)
                || !TryParseDecimal(entry.Amount, "amount", out var amount, out error)
                || !TryParseDecimal(entry.CashAmount, "cash_amount", out var cashAmount, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new RetailOrderEntryNormalized(
                OrderId: new OrderId(entry.Id),
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
        out RetailOrderEntryNormalized? normalized,
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

        normalized = new RetailOrderEntryNormalized(
            OrderId: new OrderId(entry.Id),
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


    public static PostRetailOrderPlaceResponse ToPostRetailOrderPlaceResponse(RawPrivateDtos.PostRetailOrderPlaceResponse raw)
    {
        return new PostRetailOrderPlaceResponse(
            Code: raw.Code,
            OrderId: raw.Data,
            Success: raw.Success,
            Message: ParseOptional(raw.Message));
    }

    public static PostRetailOrderCreateResponse ToPostRetailOrderCreateResponse(RawPrivateDtos.PostRetailOrderCreateResponse raw)
    {
        return new PostRetailOrderCreateResponse(
            Code: raw.Code,
            OrderId: raw.Data,
            Success: raw.Success,
            Message: ParseOptional(raw.Message));
    }

    public static PostRetailOrderCancelByOrderIdResponse ToPostRetailOrderCancelByOrderIdResponse(RawPrivateDtos.PostRetailOrderCancelByOrderIdResponse raw)
    {
        return new PostRetailOrderCancelByOrderIdResponse(
            Code: raw.Code,
            OrderId: raw.Data,
            Success: raw.Success,
            Message: ParseOptional(raw.Message));
    }

    public static PostWithdrawApiCreateResponse ToPostWithdrawApiCreateResponse(RawPrivateDtos.PostWithdrawApiCreateResponse raw)
    {
        return new PostWithdrawApiCreateResponse(FreeText.Parse(raw.Status), raw.Data);
    }

    public static PostWithdrawVirtualByAddressIdCreateResponse ToPostWithdrawVirtualByAddressIdCreateResponse(RawPrivateDtos.PostWithdrawVirtualByAddressIdCreateResponse raw)
    {
        return new PostWithdrawVirtualByAddressIdCreateResponse(FreeText.Parse(raw.Status), raw.Data);
    }

    public static PostWithdrawVirtualByWithdrawIdPlaceResponse ToPostWithdrawVirtualByWithdrawIdPlaceResponse(RawPrivateDtos.PostWithdrawVirtualByWithdrawIdPlaceResponse raw)
    {
        return new PostWithdrawVirtualByWithdrawIdPlaceResponse(FreeText.Parse(raw.Status), raw.Data);
    }

    public static PostWithdrawVirtualByWithdrawIdCancelResponse ToPostWithdrawVirtualByWithdrawIdCancelResponse(RawPrivateDtos.PostWithdrawVirtualByWithdrawIdCancelResponse raw)
    {
        return new PostWithdrawVirtualByWithdrawIdCancelResponse(FreeText.Parse(raw.Status), raw.Data);
    }

    public static bool TryToRawRetailOrder(
        RetailOrderRequest request,
        out RawPrivateRequests.RawPostRetailOrderPlaceRequest? raw,
        out CallError? error)
    {
        if (request is null)
        {
            raw = null;
            error = new CallError(CallErrorKind.Mapping, "request is required.");
            return false;
        }

        if (!ExchangeSymbol.TryParse(request.Symbol.Value, out var symbol))
        {
            raw = null;
            error = new CallError(CallErrorKind.Mapping, "Bittrade symbol is invalid.");
            return false;
        }

        raw = new RawPrivateRequests.RawPostRetailOrderPlaceRequest(
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

    private static bool TryMapExecutionSide(string? side, out OrderSide parsed, out CallError? error)
    {
        if (!OrderSideParser.TryParse(side, out parsed))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unsupported execution side: {side ?? "<null>"}.");
            return false;
        }

        error = null;
        return true;
    }

    private static bool TryMapOrderType(Side side, OrderType type, out ExchangeOrderType mapped, out CallError? error)
    {
        switch (side, type)
        {
            case (Side.Buy, OrderType.Market):
                mapped = ExchangeOrderType.BuyMarket;
                error = null;
                return true;
            case (Side.Sell, OrderType.Market):
                mapped = ExchangeOrderType.SellMarket;
                error = null;
                return true;
            case (Side.Buy, OrderType.Limit):
                mapped = ExchangeOrderType.BuyLimit;
                error = null;
                return true;
            case (Side.Sell, OrderType.Limit):
                mapped = ExchangeOrderType.SellLimit;
                error = null;
                return true;
            default:
                mapped = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order type: {type}.");
                return false;
        }
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
            case ExchangeOrderType.BuyLimit:
            case ExchangeOrderType.BuyMarket:
            case ExchangeOrderType.BuyLimitMaker:
            case ExchangeOrderType.BuyIoc:
                side = Side.Buy;
                break;
            case ExchangeOrderType.SellLimit:
            case ExchangeOrderType.SellMarket:
            case ExchangeOrderType.SellLimitMaker:
            case ExchangeOrderType.SellIoc:
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
            case ExchangeOrderType.BuyMarket:
            case ExchangeOrderType.SellMarket:
                orderType = OrderType.Market;
                break;
            case ExchangeOrderType.BuyLimit:
            case ExchangeOrderType.SellLimit:
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
            ExchangeOrderState.Submitted => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Active,
            ExchangeOrderState.PartialFilled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Active,
            ExchangeOrderState.Filled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Completed,
            ExchangeOrderState.PartialCanceled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Canceled,
            ExchangeOrderState.Canceled => ExchangeApi.Primitives.DomainCommon.Enums.OrderState.Canceled,
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

    private static bool TryToRawOrderType(ExchangeOrderType type, out string raw, out CallError? error)
    {
        raw = type switch
        {
            ExchangeOrderType.BuyLimit => "buy-limit",
            ExchangeOrderType.SellLimit => "sell-limit",
            ExchangeOrderType.BuyMarket => "buy-market",
            ExchangeOrderType.SellMarket => "sell-market",
            ExchangeOrderType.BuyLimitMaker => "buy-limit-maker",
            ExchangeOrderType.SellLimitMaker => "sell-limit-maker",
            ExchangeOrderType.BuyIoc => "buy-ioc",
            ExchangeOrderType.SellIoc => "sell-ioc",
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

    private static bool TryParseOrderType(string type, out ExchangeOrderType parsed, out CallError? error)
    {
        switch (type)
        {
            case "buy-limit":
                parsed = ExchangeOrderType.BuyLimit;
                error = null;
                return true;
            case "sell-limit":
                parsed = ExchangeOrderType.SellLimit;
                error = null;
                return true;
            case "buy-market":
                parsed = ExchangeOrderType.BuyMarket;
                error = null;
                return true;
            case "sell-market":
                parsed = ExchangeOrderType.SellMarket;
                error = null;
                return true;
            case "buy-limit-maker":
                parsed = ExchangeOrderType.BuyLimitMaker;
                error = null;
                return true;
            case "sell-limit-maker":
                parsed = ExchangeOrderType.SellLimitMaker;
                error = null;
                return true;
            case "buy-ioc":
                parsed = ExchangeOrderType.BuyIoc;
                error = null;
                return true;
            case "sell-ioc":
                parsed = ExchangeOrderType.SellIoc;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order type: {type}.");
                return false;
        }
    }

    private static Closed<ExchangeOrderType> ParseOrderTypeClosed(string? type) =>
        (type ?? string.Empty) switch
        {
            "buy-limit" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.BuyLimit),
            "sell-limit" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.SellLimit),
            "buy-market" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.BuyMarket),
            "sell-market" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.SellMarket),
            "buy-limit-maker" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.BuyLimitMaker),
            "sell-limit-maker" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.SellLimitMaker),
            "buy-ioc" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.BuyIoc),
            "sell-ioc" => Closed<ExchangeOrderType>.KnownValue(ExchangeOrderType.SellIoc),
            _ => Closed<ExchangeOrderType>.UnknownValue(type ?? string.Empty),
        };

    private static bool TryParseOrderState(string state, out ExchangeOrderState parsed, out CallError? error)
    {
        switch (state)
        {
            case "submitted":
                parsed = ExchangeOrderState.Submitted;
                error = null;
                return true;
            case "partial-filled":
                parsed = ExchangeOrderState.PartialFilled;
                error = null;
                return true;
            case "filled":
                parsed = ExchangeOrderState.Filled;
                error = null;
                return true;
            case "partial-canceled":
                parsed = ExchangeOrderState.PartialCanceled;
                error = null;
                return true;
            case "canceled":
                parsed = ExchangeOrderState.Canceled;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order state: {state}.");
                return false;
        }
    }

    private static Closed<ExchangeOrderState> ParseOrderStateClosed(string? state) =>
        (state ?? string.Empty) switch
        {
            "submitted" => Closed<ExchangeOrderState>.KnownValue(ExchangeOrderState.Submitted),
            "partial-filled" => Closed<ExchangeOrderState>.KnownValue(ExchangeOrderState.PartialFilled),
            "filled" => Closed<ExchangeOrderState>.KnownValue(ExchangeOrderState.Filled),
            "partial-canceled" => Closed<ExchangeOrderState>.KnownValue(ExchangeOrderState.PartialCanceled),
            "canceled" => Closed<ExchangeOrderState>.KnownValue(ExchangeOrderState.Canceled),
            _ => Closed<ExchangeOrderState>.UnknownValue(state ?? string.Empty),
        };

    private static string FormatDecimal(decimal value) =>
        value.ToString(CultureInfo.InvariantCulture);

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
