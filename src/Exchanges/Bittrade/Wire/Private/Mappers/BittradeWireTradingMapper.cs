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
            State: null,
            Price: request.Price,
            Size: request.Size,
            FilledSize: null,
            OutstandingSize: null,
            CreatedAt: null);

    public static BittradeWireOrder ToWire(OrderDetail raw)
    {
        var size = ParseRequiredDecimal(raw.Amount, "amount");
        var filled = ParseDecimalOrThrow(raw.FilledAmount, "field-amount") ?? 0m;
        var outstanding = Math.Max(0m, size - filled);

        return new BittradeWireOrder(
            OrderId: raw.Id.Value,
            Symbol: raw.Symbol.Value,
            Side: ToSide(raw.Type),
            Type: ToWireEnumValue(raw.Type),
            State: ToWireEnumValue(raw.State),
            Price: ParseDecimalOrThrow(raw.Price, "price"),
            Size: size,
            FilledSize: filled,
            OutstandingSize: outstanding,
            CreatedAt: raw.CreatedAt);
    }

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

        throw new FormatException($"Invalid type: '{text}'.");
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
            throw new FormatException($"Missing {field}: <missing>.");
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
            OrderType.SellLimit or OrderType.SellMarket or OrderType.SellLimitMaker or OrderType.SellIoc => "sell",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown Bittrade order type"),
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
