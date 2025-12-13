using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>親注文パラメータの執行条件 (condition_type)。</summary>
[JsonConverter(typeof(ConditionTypeJsonConverter))]
public enum ConditionType
{
    [EnumMember(Value = "LIMIT")] Limit,
    [EnumMember(Value = "MARKET")] Market,
    [EnumMember(Value = "STOP")] Stop,
    [EnumMember(Value = "STOP_LIMIT")] StopLimit,
    [EnumMember(Value = "TRAIL")] Trail,
    Unknown,
}

internal sealed class ConditionTypeJsonConverter : JsonConverter<ConditionType>
{
    public override ConditionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "LIMIT" => ConditionType.Limit,
            "MARKET" => ConditionType.Market,
            "STOP" => ConditionType.Stop,
            "STOP_LIMIT" => ConditionType.StopLimit,
            "TRAIL" => ConditionType.Trail,
            _ => ConditionType.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, ConditionType value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            ConditionType.Limit => "LIMIT",
            ConditionType.Market => "MARKET",
            ConditionType.Stop => "STOP",
            ConditionType.StopLimit => "STOP_LIMIT",
            ConditionType.Trail => "TRAIL",
            _ => "LIMIT",
        };
        writer.WriteStringValue(str);
    }
}
