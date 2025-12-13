using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>bitFlyer child order type (LIMIT / MARKET)。</summary>
[JsonConverter(typeof(ChildOrderTypeJsonConverter))]
public enum ChildOrderType
{
    [EnumMember(Value = "MARKET")]
    Market,

    [EnumMember(Value = "LIMIT")]
    Limit,

    Unknown
}

internal sealed class ChildOrderTypeJsonConverter : JsonConverter<ChildOrderType>
{
    public override ChildOrderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "MARKET" => ChildOrderType.Market,
            "LIMIT" => ChildOrderType.Limit,
            _ => ChildOrderType.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, ChildOrderType value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            ChildOrderType.Market => "MARKET",
            ChildOrderType.Limit => "LIMIT",
            _ => "MARKET",
        };
        writer.WriteStringValue(str);
    }
}
