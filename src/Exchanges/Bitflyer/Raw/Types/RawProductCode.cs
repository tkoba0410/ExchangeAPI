using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Types;

[JsonConverter(typeof(RawProductCodeJsonConverter))]
public readonly record struct RawProductCode(string Value)
{
    public override string ToString() => Value;

    public static implicit operator string(RawProductCode v) => v.Value;
    public static implicit operator RawProductCode(string v) => new(v);
}

public sealed class RawProductCodeJsonConverter : JsonConverter<RawProductCode>
{
    public override RawProductCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is null) throw new JsonException("product_code is null");
        return new RawProductCode(s);
    }

    public override void Write(Utf8JsonWriter writer, RawProductCode value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
