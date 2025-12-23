using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private.Mappers;

internal static class BittradeWireTradingMapper
{
    public static CreateOrderRequest ToRaw(string accountId, BittradeWireCreateOrderRequest wire) =>
        new(
            AccountId: accountId,
            Symbol: Symbol.From(wire.Symbol),
            Type: ParseOrderType(wire.Type),
            Amount: FormatDecimal(wire.Size),
            Price: wire.Price is null ? null : FormatDecimal(wire.Price.Value),
            Source: null);

    public static BittradeWireOrder ToWire(PlaceOrderResponse raw, BittradeWireCreateOrderRequest request)
        => new(
            OrderId: raw.OrderId.Value,
            Symbol: request.Symbol,
            Side: request.Side,
            Type: request.Type,
            Price: request.Price,
            Size: request.Size,
            CreatedAt: null);

    public static BittradeWireOrder ToWire(OrderDetail raw)
        => new(
            OrderId: raw.Id.Value,
            Symbol: raw.Symbol.Value,
            Side: ToSide(raw.Type),
            Type: ToWireEnumValue(raw.Type),
            Price: ParseDecimalOrThrow(raw.Price, "price"),
            Size: ParseRequiredDecimal(raw.Amount, "amount"),
            CreatedAt: raw.CreatedAt);

    public static BittradeWireOpenOrder ToWireOpenOrder(OrderSummary raw)
        => new(
            OrderId: raw.Id.Value,
            Symbol: raw.Symbol.Value,
            Side: ToSide(raw.Type),
            Type: ToWireEnumValue(raw.Type),
            State: ToWireEnumValue(raw.State),
            Price: ParseDecimalOrThrow(raw.Price, "price"),
            Size: ParseRequiredDecimal(raw.Amount, "amount"),
            FilledSize: ParseDecimalOrThrow(raw.FilledAmount, "filled-amount") ?? 0m,
            CreatedAt: raw.CreatedAt);

    private static OrderType ParseOrderType(string text)
    {
        if (Enum.TryParse<OrderType>(NormalizeEnumKey(text), ignoreCase: true, out var result))
        {
            return result;
        }

        throw new FormatException($"Unknown order type: '{text}'.");
    }

    private static string NormalizeEnumKey(string text)
        => text.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

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

        throw new FormatException($"Invalid {field}: '{text}'.");
    }

    private static decimal ParseRequiredDecimal(string? text, string field)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new FormatException($"Missing {field}.");
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new FormatException($"Invalid {field}: '{text}'.");
    }

    private static string ToSide(OrderType type) =>
        type switch
        {
            OrderType.BuyLimit or OrderType.BuyMarket or OrderType.BuyLimitMaker or OrderType.BuyIoc => "buy",
            _ => "sell",
        };

    private static string ToWireEnumValue<T>(T value)
        where T : struct, Enum
    {
        var name = Enum.GetName(value) ?? value.ToString();
        var member = typeof(T).GetMember(name).FirstOrDefault();
        var enumMember = member?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
            .OfType<EnumMemberAttribute>()
            .FirstOrDefault();
        return enumMember?.Value ?? name;
    }
}
