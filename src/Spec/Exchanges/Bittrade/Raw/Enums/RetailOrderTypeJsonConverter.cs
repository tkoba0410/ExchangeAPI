using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed class RetailOrderTypeJsonConverter : JsonConverter<RetailOrderType>
{
    public override RetailOrderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException($"Expected number for {nameof(RetailOrderType)}.");
        }

        var value = reader.GetInt32();
        if (!Enum.IsDefined(typeof(RetailOrderType), value))
        {
            throw new JsonException($"Unknown {nameof(RetailOrderType)} value: {value}.");
        }

        return (RetailOrderType)value;
    }

    public override void Write(Utf8JsonWriter writer, RetailOrderType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}
