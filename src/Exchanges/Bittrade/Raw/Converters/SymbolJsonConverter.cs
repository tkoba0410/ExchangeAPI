using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class SymbolJsonConverter : JsonConverter<Symbol>
{
    public override Symbol Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string for symbol.");
        }

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("Symbol must not be empty.");
        }

        return new Symbol(value);
    }

    public override void Write(Utf8JsonWriter writer, Symbol value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
