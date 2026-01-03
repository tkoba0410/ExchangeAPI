using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class SymbolJsonConverter : JsonConverter<RawSymbol>
{
    public override RawSymbol Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string for symbol.");
        }

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("RawSymbol must not be empty.");
        }

        return new RawSymbol(value);
    }

    public override void Write(Utf8JsonWriter writer, RawSymbol value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
