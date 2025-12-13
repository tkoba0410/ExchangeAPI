using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>bitFlyer プロダクトコード。</summary>
[JsonConverter(typeof(ProductCodeJsonConverter))]
public enum ProductCode
{
    [EnumMember(Value = "BTC_JPY")]
    BtcJpy,

    [EnumMember(Value = "FX_BTC_JPY")]
    FxBtcJpy,

    Unknown
}

internal sealed class ProductCodeJsonConverter : JsonConverter<ProductCode>
{
    public override ProductCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "BTC_JPY" => ProductCode.BtcJpy,
            "FX_BTC_JPY" => ProductCode.FxBtcJpy,
            _ => ProductCode.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, ProductCode value, JsonSerializerOptions options)
    {
        var str = value switch
        {
            ProductCode.BtcJpy => "BTC_JPY",
            ProductCode.FxBtcJpy => "FX_BTC_JPY",
            _ => "BTC_JPY",
        };
        writer.WriteStringValue(str);
    }
}
