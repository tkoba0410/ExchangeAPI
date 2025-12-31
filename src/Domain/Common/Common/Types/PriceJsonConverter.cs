using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Common.Types;

public sealed class PriceJsonConverter : JsonConverter<Price>
{
    public override Price Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return new Price(reader.GetDecimal());
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new JsonException("Price must not be empty.");
            }

            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                throw new JsonException($"Invalid price: '{text}'.");
            }

            return new Price(value);
        }

        throw new JsonException("Expected number or string for price.");
    }

    public override void Write(Utf8JsonWriter writer, Price value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}
