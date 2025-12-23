using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

/// <summary>bitFlyer time in force (GTC/IOC/FOK)。</summary>
[JsonConverter(typeof(TimeInForceJsonConverter))]
public enum TimeInForce
{
    [EnumMember(Value = "GTC")]
    Gtc,

    [EnumMember(Value = "IOC")]
    Ioc,

    [EnumMember(Value = "FOK")]
    Fok,

    Unknown
}

internal sealed class TimeInForceJsonConverter : JsonConverter<TimeInForce>
{
    public override TimeInForce Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "GTC" => TimeInForce.Gtc,
            "IOC" => TimeInForce.Ioc,
            "FOK" => TimeInForce.Fok,
            _ => TimeInForce.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, TimeInForce value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            TimeInForce.Gtc => "GTC",
            TimeInForce.Ioc => "IOC",
            TimeInForce.Fok => "FOK",
            _ => "GTC",
        };
        writer.WriteStringValue(str);
    }
}
