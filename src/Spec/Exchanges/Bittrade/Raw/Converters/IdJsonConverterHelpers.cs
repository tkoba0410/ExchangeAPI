using System;
using System.Globalization;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal static class IdJsonConverterHelpers
{
    public static string ReadStringOrNumber(ref Utf8JsonReader reader)
    {
        string? value = reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.GetInt64().ToString(CultureInfo.InvariantCulture),
            _ => throw new JsonException("Expected string or number for id.")
        };

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("Id must not be empty.");
        }

        return value;
    }

    public static long ReadLong(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException("Expected number for id.");
        }

        return reader.GetInt64();
    }
}
