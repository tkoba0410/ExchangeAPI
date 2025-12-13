using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>bitFlyer child order type (LIMIT/MARKET/STOP/STOP_LIMIT/TRAIL)。</summary>
[JsonConverter(typeof(ChildOrderTypeJsonConverter))]
public enum ChildOrderType
{
    [EnumMember(Value = "MARKET")]
    Market,

    [EnumMember(Value = "LIMIT")]
    Limit,

    [EnumMember(Value = "STOP")]
    Stop,

    [EnumMember(Value = "STOP_LIMIT")]
    StopLimit,

    [EnumMember(Value = "TRAIL")]
    Trail,

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
            "STOP" => ChildOrderType.Stop,
            "STOP_LIMIT" => ChildOrderType.StopLimit,
            "TRAIL" => ChildOrderType.Trail,
            _ => ChildOrderType.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, ChildOrderType value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            ChildOrderType.Market => "MARKET",
            ChildOrderType.Limit => "LIMIT",
            ChildOrderType.Stop => "STOP",
            ChildOrderType.StopLimit => "STOP_LIMIT",
            ChildOrderType.Trail => "TRAIL",
            _ => "MARKET",
        };
        writer.WriteStringValue(str);
    }
}
