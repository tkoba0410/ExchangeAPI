using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;

internal static class BittradeTradingMapper
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static RawPrivateRequests.RawCreateOrderRequest ToRaw(string accountId, string apiSymbol, BittradeOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        if (string.IsNullOrWhiteSpace(apiSymbol))
        {
            throw new ArgumentException("apiSymbol is required.", nameof(apiSymbol));
        }

        var type = ToRawOrderType(MapOrderType(request.Side, request.OrderType));
        var price = request.Price?.Value;
        var size = FormatDecimal(request.Size.Value);

        return new RawPrivateRequests.RawCreateOrderRequest(
            AccountId: accountId,
            Symbol: apiSymbol,
            Type: type,
            Amount: size,
            Price: price is null ? null : FormatDecimal(price.Value),
            Source: null);
    }

    public static BittradeOrderResult ToOrderResult(RawPrivateDtos.RawPlaceOrderResponse raw)
    {
        var orderId = raw.OrderId;
        var key = new OrderKey(OrderIdKind.ExchangeOrderId, orderId);
        return new BittradeOrderResult(key, ExchangeOrderId: orderId);
    }

    public static IReadOnlyList<BittradeOpenOrder> ToOpenOrders(Symbol symbol, RawPrivateDtos.RawOpenOrdersResponse raw)
    {
        if (raw.Data is null || raw.Data.Count == 0)
        {
            return Array.Empty<BittradeOpenOrder>();
        }

        return raw.Data.Select(order => ToOpenOrder(symbol, order)).ToList();
    }

    private static BittradeOpenOrder ToOpenOrder(Symbol symbol, RawPrivateDtos.RawOrderSummary raw)
    {
        var (side, type) = MapSideAndType(raw.Type);
        var status = MapStatus(raw.State);
        var size = new Size(ParseRequiredDecimal(raw.Amount, "amount"));
        var executed = new Size(ParseDecimalOrThrow(raw.FilledAmount, "field-amount") ?? 0m);
        var outstanding = new Size(Math.Max(0m, size.Value - executed.Value));
        var priceValue = ParseDecimalOrThrow(raw.Price, "price");
        var price = priceValue is null ? (Price?)null : new Price(priceValue.Value);

        return new BittradeOpenOrder(
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
            Status: raw.State,
            ExchangeOrderId: raw.Id);
    }

    public static BittradeOrderStatus ToOrderStatus(string productCode, RawPrivateDtos.RawOrderDetailResponse raw, OrderKey key)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        if (raw.Data is null)
        {
            throw new InvalidOperationException("Bittrade order response is missing data.");
        }

        var status = MapStatus(raw.Data.State);
        var priceValue = ParseDecimalOrThrow(raw.Data.Price, "price");
        var price = priceValue is null ? (Price?)null : new Price(priceValue.Value);
        var size = ParseRequiredDecimal(raw.Data.Amount, "amount");
        var executed = new Size(ParseDecimalOrThrow(raw.Data.FilledAmount, "field-amount") ?? 0m);
        var outstanding = new Size(Math.Max(0m, size - executed.Value));

        return new BittradeOrderStatus(
            productCode,
            key,
            status,
            executed,
            outstanding,
            price,
            null);
    }

    public static IReadOnlyList<BittradeExecutionNormalized> ToExecutions(
        IReadOnlyList<RawPrivateDtos.RawMatchResultEntry> entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeExecutionNormalized>();
        }

        var snapshots = entries
            .Select(entry => ExtractSnapshot(Serialize(entry)))
            .ToArray();

        return entries
            .Select((entry, idx) => new BittradeExecutionNormalized(
                Id: string.IsNullOrWhiteSpace(entry.MatchId) ? entry.Id : entry.MatchId,
                Side: MapSide(entry.Type),
                Price: entry.Price,
                Size: entry.FilledAmount,
                Timestamp: entry.CreatedAt,
                RawSnapshot: snapshots[idx],
                Extras: new Dictionary<string, JsonElement>()))
            .ToList();
    }

    public static IReadOnlyList<BittradeOrderSummaryNormalized> ToOrderSummaries(
        IReadOnlyList<RawPrivateDtos.RawOrderSummary>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeOrderSummaryNormalized>();
        }

        return entries
            .Select(entry => new BittradeOrderSummaryNormalized(
                Id: entry.Id,
                Symbol: entry.Symbol,
                AccountId: entry.AccountId,
                Amount: ParseRequiredDecimal(entry.Amount, "amount"),
                Price: ParseDecimalOrThrow(entry.Price, "price"),
                State: entry.State,
                Type: entry.Type,
                ClientOrderId: entry.ClientOrderId,
                CreatedAt: entry.CreatedAt,
                FilledAmount: ParseRequiredDecimal(entry.FilledAmount, "field-amount")))
            .ToList();
    }

    public static IReadOnlyList<BittradeRetailOrderEntryNormalized> ToRetailOrders(
        IReadOnlyList<RawPrivateDtos.RawRetailOrderEntry>? entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeRetailOrderEntryNormalized>();
        }

        return entries
            .Select(entry => new BittradeRetailOrderEntryNormalized(
                Id: entry.Id,
                Symbol: entry.Symbol,
                Type: entry.Type,
                Price: ParseDecimalOrThrow(entry.Price, "price"),
                Amount: ParseDecimalOrThrow(entry.Amount, "amount"),
                CashAmount: ParseDecimalOrThrow(entry.CashAmount, "cash_amount"),
                Status: entry.Status,
                CreatedAt: entry.CreatedAt))
            .ToList();
    }

    public static BittradeRetailOrderEntryNormalized? ToRetailOrder(RawPrivateDtos.RawRetailOrderEntry? entry)
    {
        if (entry is null)
        {
            return null;
        }

        return new BittradeRetailOrderEntryNormalized(
            Id: entry.Id,
            Symbol: entry.Symbol,
            Type: entry.Type,
            Price: ParseDecimalOrThrow(entry.Price, "price"),
            Amount: ParseDecimalOrThrow(entry.Amount, "amount"),
            CashAmount: ParseDecimalOrThrow(entry.CashAmount, "cash_amount"),
            Status: entry.Status,
            CreatedAt: entry.CreatedAt);
    }

    public static BittradeRetailOrderResult ToRetailOrderResult(RawPrivateDtos.RawRetailOrderResponse raw)
    {
        return new BittradeRetailOrderResult(
            Code: raw.Code,
            OrderId: raw.Data,
            Success: raw.Success,
            Message: raw.Message);
    }

    public static BittradeWithdrawResult ToWithdrawResult(RawPrivateDtos.RawCreateWithdrawResponse raw)
    {
        return new BittradeWithdrawResult(raw.Status, raw.Data);
    }

    public static BittradeWithdrawResult ToWithdrawResult(RawPrivateDtos.RawCancelWithdrawResponse raw)
    {
        return new BittradeWithdrawResult(raw.Status, raw.Data);
    }

    public static RawPrivateRequests.RawCreateRetailOrderRequest ToRawRetailOrder(BittradeRetailOrderRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var apiSymbol = BittradeSymbol.Normalize(request.Symbol.Value);
        return new RawPrivateRequests.RawCreateRetailOrderRequest(
            Symbol: apiSymbol,
            Type: request.Type,
            Price: request.Price is null ? null : FormatDecimal(request.Price.Value),
            Amount: request.Amount is null ? null : FormatDecimal(request.Amount.Value),
            CashAmount: request.CashAmount is null ? null : FormatDecimal(request.CashAmount.Value));
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

}
