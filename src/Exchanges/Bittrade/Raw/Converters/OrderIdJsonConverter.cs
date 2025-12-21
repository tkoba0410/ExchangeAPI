using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class OrderIdJsonConverter : JsonConverter<OrderId>
{
    public override OrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => new OrderId(reader.GetString() ?? throw new JsonException("OrderId null.")),
            JsonTokenType.Number => new OrderId(reader.GetInt64().ToString(CultureInfo.InvariantCulture)),
            _ => throw new JsonException("Expected string or number for OrderId.")
        };
    }

    public override void Write(Utf8JsonWriter writer, OrderId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
