using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

[JsonConverter(typeof(BittradeOrderTypeJsonConverter))]
public enum BittradeOrderType
{
    [EnumMember(Value = "buy-limit")]
    BuyLimit,
    [EnumMember(Value = "sell-limit")]
    SellLimit,
    [EnumMember(Value = "buy-market")]
    BuyMarket,
    [EnumMember(Value = "sell-market")]
    SellMarket,
    [EnumMember(Value = "buy-limit-maker")]
    BuyLimitMaker,
    [EnumMember(Value = "sell-limit-maker")]
    SellLimitMaker,
    [EnumMember(Value = "buy-ioc")]
    BuyIoc,
    [EnumMember(Value = "sell-ioc")]
    SellIoc
}

[JsonConverter(typeof(BittradeOrderStateJsonConverter))]
public enum BittradeOrderState
{
    [EnumMember(Value = "submitted")]
    Submitted,
    [EnumMember(Value = "partial-filled")]
    PartialFilled,
    [EnumMember(Value = "partial-canceled")]
    PartialCanceled,
    [EnumMember(Value = "filled")]
    Filled,
    [EnumMember(Value = "canceled")]
    Canceled
}

[JsonConverter(typeof(BittradeOrderSideJsonConverter))]
public enum BittradeOrderSide
{
    [EnumMember(Value = "buy")]
    Buy,
    [EnumMember(Value = "sell")]
    Sell
}

[JsonConverter(typeof(BittradeRetailOrderTypeJsonConverter))]
public enum BittradeRetailOrderType
{
    Buy = 1,
    Sell = 2
}

public sealed class BittradeOrderTypeJsonConverter : StrictStringEnumConverter<BittradeOrderType>
{
}

public sealed class BittradeOrderStateJsonConverter : StrictStringEnumConverter<BittradeOrderState>
{
}

public sealed class BittradeOrderSideJsonConverter : StrictStringEnumConverter<BittradeOrderSide>
{
}

public sealed class BittradeRetailOrderTypeJsonConverter : JsonConverter<BittradeRetailOrderType>
{
    public override BittradeRetailOrderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException($"Expected number for {nameof(BittradeRetailOrderType)}.");
        }

        var value = reader.GetInt32();
        if (!Enum.IsDefined(typeof(BittradeRetailOrderType), value))
        {
            throw new JsonException($"Unknown {nameof(BittradeRetailOrderType)} value: {value}.");
        }

        return (BittradeRetailOrderType)value;
    }

    public override void Write(Utf8JsonWriter writer, BittradeRetailOrderType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}

public abstract class StrictStringEnumConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    private readonly IReadOnlyDictionary<string, T> _byValue;
    private readonly IReadOnlyDictionary<T, string> _byEnum;

    protected StrictStringEnumConverter()
    {
        var values = Enum.GetValues<T>();
        var byValue = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        var byEnum = new Dictionary<T, string>();
        foreach (var value in values)
        {
            var name = Enum.GetName(value) ?? value.ToString();
            var member = typeof(T).GetMember(name).FirstOrDefault();
            var enumMember = member?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .OfType<EnumMemberAttribute>()
                .FirstOrDefault();
            var wire = enumMember?.Value ?? name;
            byValue[wire] = value;
            byEnum[value] = wire;
        }

        _byValue = byValue;
        _byEnum = byEnum;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {typeof(T).Name}.");
        }

        var value = reader.GetString();
        if (value is null || !_byValue.TryGetValue(value, out var result))
        {
            throw new JsonException($"Unknown {typeof(T).Name} value: {value ?? "<null>"}.");
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (!_byEnum.TryGetValue(value, out var text))
        {
            throw new JsonException($"Unknown {typeof(T).Name} value: {value}.");
        }

        writer.WriteStringValue(text);
    }
}
