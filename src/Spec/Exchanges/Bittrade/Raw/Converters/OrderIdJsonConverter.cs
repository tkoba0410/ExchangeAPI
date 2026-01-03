using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class OrderIdJsonConverter : JsonConverter<RawOrderId>
{
    public override RawOrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => new RawOrderId(reader.GetString() ?? throw new JsonException("RawOrderId null.")),
            JsonTokenType.Number => new RawOrderId(reader.GetInt64().ToString(CultureInfo.InvariantCulture)),
            _ => throw new JsonException("Expected string or number for RawOrderId.")
        };
    }

    public override void Write(Utf8JsonWriter writer, RawOrderId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
