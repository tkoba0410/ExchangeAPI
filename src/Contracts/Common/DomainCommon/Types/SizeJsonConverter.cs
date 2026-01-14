using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Contracts.Common.DomainCommon.Types;

public sealed class SizeJsonConverter : JsonConverter<Size>
{
    public override Size Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return new Size(reader.GetDecimal());
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new JsonException("Size must not be empty.");
            }

            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                throw new JsonException($"Invalid size: '{text}'.");
            }

            return new Size(value);
        }

        throw new JsonException("Expected number or string for size.");
    }

    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}
