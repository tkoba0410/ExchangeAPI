using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;
using ContractOrderType = ExchangeApi.Common.Enums.OrderType;
using RawOrderState = ExchangeApi.Exchanges.Bittrade.Raw.OrderState;
using RawOrderType = ExchangeApi.Exchanges.Bittrade.Raw.OrderType;
using ExchangeApi.Exchanges.Bittrade.Raw;

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

        var type = MapOrderType(request.Side, request.OrderType);
        var price = request.Price?.Value;
        var size = FormatDecimal(request.Size.Value);

        return new RawCreateOrderRequest(
            AccountId: accountId,
            RawSymbol: RawSymbol.From(apiSymbol),
            Type: type,
            Amount: size,
            Price: price is null ? null : FormatDecimal(price.Value),
            Source: null);
    }

    public static OrderResult ToOrderResult(RawPlaceOrderResponse raw)
    {
        var orderId = raw.RawOrderId.Value;
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
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, raw.Id.Value),
            Side: side,
            OrderType: type,
            Size: size,
            OutstandingSize: outstanding,
            ExecutedSize: executed,
            Price: price,
            OrderedAt: raw.CreatedAt,
            UpdatedAt: null,
            StopPrice: null,
            Status: ToExchangeEnumValue(raw.State),
            ExchangeOrderId: raw.Id.Value);
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

    private static RawOrderType MapOrderType(Side side, ContractOrderType type)
    {
        return (side, type) switch
        {
            (Side.Buy, ContractOrderType.Market) => RawOrderType.BuyMarket,
            (Side.Sell, ContractOrderType.Market) => RawOrderType.SellMarket,
            (Side.Buy, ContractOrderType.Limit) => RawOrderType.BuyLimit,
            (Side.Sell, ContractOrderType.Limit) => RawOrderType.SellLimit,
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
        };
    }

    private static (Side Side, ContractOrderType OrderType) MapSideAndType(RawOrderType type)
    {
        var parsedSide = type switch
        {
            RawOrderType.BuyLimit or RawOrderType.BuyMarket or RawOrderType.BuyLimitMaker or RawOrderType.BuyIoc => Side.Buy,
            RawOrderType.SellLimit or RawOrderType.SellMarket or RawOrderType.SellLimitMaker or RawOrderType.SellIoc => Side.Sell,
            _ => throw new ExchangeApiException($"Unsupported order side: {type}.", exchange: Exchange)
        };

        var parsedType = type switch
        {
            RawOrderType.BuyMarket or RawOrderType.SellMarket => ContractOrderType.Market,
            RawOrderType.BuyLimit or RawOrderType.SellLimit => ContractOrderType.Limit,
            _ => throw new ExchangeApiException($"Unsupported order type: {type}.", exchange: Exchange)
        };

        return (parsedSide, parsedType);
    }

    private static ExchangeApi.Common.Enums.OrderState MapStatus(RawOrderState state)
    {
        return state switch
        {
            RawOrderState.Submitted => ExchangeApi.Common.Enums.OrderState.Active,
            RawOrderState.PartialFilled => ExchangeApi.Common.Enums.OrderState.Active,
            RawOrderState.Filled => ExchangeApi.Common.Enums.OrderState.Completed,
            RawOrderState.PartialCanceled => ExchangeApi.Common.Enums.OrderState.Canceled,
            RawOrderState.Canceled => ExchangeApi.Common.Enums.OrderState.Canceled,
            _ => throw new ExchangeApiException($"Unsupported order state: {state}.", exchange: Exchange)
        };
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

    private static string ToExchangeEnumValue<T>(T value)
        where T : struct, Enum
    {
        var name = Enum.GetName(value) ?? value.ToString();
        var member = typeof(T).GetMember(name).FirstOrDefault();
        var enumMember = member?.GetCustomAttributes(typeof(System.Runtime.Serialization.EnumMemberAttribute), false)
            .OfType<System.Runtime.Serialization.EnumMemberAttribute>()
            .FirstOrDefault();
        return enumMember?.Value ?? name;
    }
}
