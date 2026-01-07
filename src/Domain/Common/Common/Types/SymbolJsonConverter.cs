using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Common.Types;

public sealed class SymbolJsonConverter : JsonConverter<Symbol>
{
    public override Symbol Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new Symbol(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for Symbol.");
    }

    public override void Write(Utf8JsonWriter writer, Symbol value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
