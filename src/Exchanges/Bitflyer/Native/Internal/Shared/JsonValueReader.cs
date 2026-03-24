using System.Globalization;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;

internal static class JsonValueReader
{
    internal static JsonElement EnsureObject(string? bodyText)
    {
        if (string.IsNullOrWhiteSpace(bodyText))
        {
            throw new CodecException("Response body is empty.");
        }

        using var document = JsonDocument.Parse(bodyText);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new CodecException("Expected top-level object.");
        }

        return document.RootElement.Clone();
    }

    internal static JsonElement EnsureArray(string? bodyText)
    {
        if (string.IsNullOrWhiteSpace(bodyText))
        {
            throw new CodecException("Response body is empty.");
        }

        using var document = JsonDocument.Parse(bodyText);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new CodecException("Expected top-level array.");
        }

        return document.RootElement.Clone();
    }

    internal static string ReadRequiredString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            throw new CodecException($"Missing required property '{propertyName}'.");
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new CodecException($"Property '{propertyName}' must be a string.");
        }

        return property.GetString() ?? throw new CodecException($"Property '{propertyName}' must not be null.");
    }

    internal static decimal ReadRequiredDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            throw new CodecException($"Missing required property '{propertyName}'.");
        }

        if (property.ValueKind != JsonValueKind.Number || !property.TryGetDecimal(out var value))
        {
            throw new CodecException($"Property '{propertyName}' must be a decimal number.");
        }

        return value;
    }

    internal static long ReadRequiredLong(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            throw new CodecException($"Missing required property '{propertyName}'.");
        }

        if (property.ValueKind != JsonValueKind.Number || !property.TryGetInt64(out var value))
        {
            throw new CodecException($"Property '{propertyName}' must be an integer number.");
        }

        return value;
    }

    internal static DateTimeOffset ReadRequiredTimestamp(JsonElement element, string propertyName)
    {
        var raw = ReadRequiredString(element, propertyName);
        if (DateTimeOffset.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var value))
        {
            return value;
        }

        throw new CodecException($"Property '{propertyName}' must be a timestamp.");
    }
}
