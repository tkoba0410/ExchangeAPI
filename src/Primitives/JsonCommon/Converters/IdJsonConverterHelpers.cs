using System;
using System.Globalization;
using System.Text.Json;

namespace ExchangeApi.Primitives.JsonCommon.Converters;

internal static class IdJsonConverterHelpers
{
    public static string ReadStringOrNumber(ref Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.Number => reader.GetDecimal().ToString(CultureInfo.InvariantCulture),
            JsonTokenType.Null => string.Empty,
            _ => throw new JsonException("Expected string or number for id.")
        };
    }
}
