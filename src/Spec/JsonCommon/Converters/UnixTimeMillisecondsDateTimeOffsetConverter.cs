using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Spec.JsonCommon.Converters;

public sealed class UnixTimeMillisecondsDateTimeOffsetConverter : ReadOnlyJsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException("Expected number for unix time milliseconds.");
        }

        var value = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(value);
    }

}
