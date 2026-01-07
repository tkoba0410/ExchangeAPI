using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ContractOrderType = ExchangeApi.Common.Enums.OrderType;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalize.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;

internal static class BittradeTradingMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public static RawCreateOrderRequest ToRaw(string accountId, string apiSymbol, OrderRequest request)
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

        return new RawCreateOrderRequest(
            AccountId: accountId,
            Symbol: apiSymbol,
            Type: type,
            Amount: size,
            Price: price is null ? null : FormatDecimal(price.Value),
            Source: null);
    }

    public static OrderResult ToOrderResult(RawPlaceOrderResponse raw)
    {
        var orderId = raw.OrderId;
        var key = new OrderKey(OrderIdKind.ExchangeOrderId, orderId);
        return new OrderResult(key, ExchangeOrderId: orderId);
    }

    public static IReadOnlyList<OpenOrder> ToOpenOrders(Symbol symbol, RawOpenOrdersResponse raw)
    {
        if (raw.Data is null || raw.Data.Count == 0)
        {
            return Array.Empty<OpenOrder>();
        }

        return raw.Data.Select(order => ToOpenOrder(symbol, order)).ToList();
    }

    private static OpenOrder ToOpenOrder(Symbol symbol, RawOrderSummary raw)
    {
        var (side, type) = MapSideAndType(raw.Type);
        var status = MapStatus(raw.State);
        var size = new Size(ParseRequiredDecimal(raw.Amount, "amount"));
        var executed = new Size(ParseDecimalOrThrow(raw.FilledAmount, "field-amount") ?? 0m);
        var outstanding = new Size(Math.Max(0m, size.Value - executed.Value));
        var priceValue = ParseDecimalOrThrow(raw.Price, "price");
        var price = priceValue is null ? (Price?)null : new Price(priceValue.Value);

        return new OpenOrder(
            ExchangeCode: Exchange,
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

    public static OrderStatus ToOrderStatus(string productCode, RawOrderDetailResponse raw, OrderKey key)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        if (raw.Data is null)
        {
            throw new ExchangeApiException("Bittrade order response is missing data.", exchange: Exchange);
        }

        var status = MapStatus(raw.Data.State);
        var priceValue = ParseDecimalOrThrow(raw.Data.Price, "price");
        var price = priceValue is null ? (Price?)null : new Price(priceValue.Value);
        var size = ParseRequiredDecimal(raw.Data.Amount, "amount");
        var executed = new Size(ParseDecimalOrThrow(raw.Data.FilledAmount, "field-amount") ?? 0m);
        var outstanding = new Size(Math.Max(0m, size - executed.Value));

        return new OrderStatus(
            productCode,
            key,
            status,
            executed,
            outstanding,
            price,
            null);
    }

    public static IReadOnlyList<BittradeExecutionNormalized> ToExecutions(
        IReadOnlyList<RawMatchResultEntry> entries)
    {
        if (entries is null || entries.Count == 0)
        {
            return Array.Empty<BittradeExecutionNormalized>();
        }

        return entries
            .Select(entry => new BittradeExecutionNormalized(
                Id: string.IsNullOrWhiteSpace(entry.MatchId) ? entry.Id : entry.MatchId,
                Side: entry.Type,
                Price: entry.Price,
                Size: entry.FilledAmount,
                Timestamp: entry.CreatedAt))
            .ToList();
    }

    private static BittradeOrderType MapOrderType(Side side, ContractOrderType type)
    {
        return (side, type) switch
        {
            (Side.Buy, ContractOrderType.Market) => BittradeOrderType.BuyMarket,
            (Side.Sell, ContractOrderType.Market) => BittradeOrderType.SellMarket,
            (Side.Buy, ContractOrderType.Limit) => BittradeOrderType.BuyLimit,
            (Side.Sell, ContractOrderType.Limit) => BittradeOrderType.SellLimit,
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
        };
    }

    private static (Side Side, ContractOrderType OrderType) MapSideAndType(string type)
    {
        var parsedType = ParseOrderType(type);
        var parsedSide = parsedType switch
        {
            BittradeOrderType.BuyLimit or BittradeOrderType.BuyMarket or BittradeOrderType.BuyLimitMaker or BittradeOrderType.BuyIoc => Side.Buy,
            BittradeOrderType.SellLimit or BittradeOrderType.SellMarket or BittradeOrderType.SellLimitMaker or BittradeOrderType.SellIoc => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported order side: {type}.", exchange: Exchange)
        };

        var orderType = parsedType switch
        {
            BittradeOrderType.BuyMarket or BittradeOrderType.SellMarket => ContractOrderType.Market,
            BittradeOrderType.BuyLimit or BittradeOrderType.SellLimit => ContractOrderType.Limit,
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
        };

        return (parsedSide, orderType);
    }

    private static ExchangeApi.Common.Enums.OrderState MapStatus(string state)
    {
        return ParseOrderState(state) switch
        {
            BittradeOrderState.Submitted => ExchangeApi.Common.Enums.OrderState.Active,
            BittradeOrderState.PartialFilled => ExchangeApi.Common.Enums.OrderState.Active,
            BittradeOrderState.Filled => ExchangeApi.Common.Enums.OrderState.Completed,
            BittradeOrderState.PartialCanceled => ExchangeApi.Common.Enums.OrderState.Canceled,
            BittradeOrderState.Canceled => ExchangeApi.Common.Enums.OrderState.Canceled,
            _ => throw new ExchangeApiException($"Unsupported order state: {state}.", exchange: Exchange)
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
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
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
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
        };

    private static BittradeOrderState ParseOrderState(string state) =>
        state switch
        {
            "submitted" => BittradeOrderState.Submitted,
            "partial-filled" => BittradeOrderState.PartialFilled,
            "filled" => BittradeOrderState.Filled,
            "partial-canceled" => BittradeOrderState.PartialCanceled,
            "canceled" => BittradeOrderState.Canceled,
            _ => throw new ExchangeApiException($"Unsupported order state: {state}.", exchange: Exchange)
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

        throw new ExchangeApiException($"Invalid {field}: '{text}'.", exchange: Exchange);
    }

    private static decimal ParseRequiredDecimal(string? text, string field)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ExchangeApiException($"Missing {field}: <missing>.", exchange: Exchange);
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new ExchangeApiException($"Invalid {field}: '{text}'.", exchange: Exchange);
    }

}
