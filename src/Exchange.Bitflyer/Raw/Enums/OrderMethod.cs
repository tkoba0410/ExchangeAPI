using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>親注文の order_method (SIMPLE/IFD/OCO/IFDOCO)。</summary>
[JsonConverter(typeof(OrderMethodJsonConverter))]
public enum OrderMethod
{
    [EnumMember(Value = "SIMPLE")]
    Simple,

    [EnumMember(Value = "IFD")]
    Ifd,

    [EnumMember(Value = "OCO")]
    Oco,

    [EnumMember(Value = "IFDOCO")]
    IfdOco,

    Unknown
}

internal sealed class OrderMethodJsonConverter : JsonConverter<OrderMethod>
{
    public override OrderMethod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "SIMPLE" => OrderMethod.Simple,
            "IFD" => OrderMethod.Ifd,
            "OCO" => OrderMethod.Oco,
            "IFDOCO" => OrderMethod.IfdOco,
            _ => OrderMethod.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, OrderMethod value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            OrderMethod.Simple => "SIMPLE",
            OrderMethod.Ifd => "IFD",
            OrderMethod.Oco => "OCO",
            OrderMethod.IfdOco => "IFDOCO",
            _ => "SIMPLE",
        };
        writer.WriteStringValue(str);
    }
}
