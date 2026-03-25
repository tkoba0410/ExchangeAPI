using System.Globalization;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;

internal static class JsonValueReader
{
    private static readonly TimeSpan JstOffset = TimeSpan.FromHours(9);

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

    internal static bool ReadRequiredBoolean(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            throw new CodecException($"Missing required property '{propertyName}'.");
        }

        if (property.ValueKind is not JsonValueKind.True and not JsonValueKind.False)
        {
            throw new CodecException($"Property '{propertyName}' must be a boolean.");
        }

        return property.GetBoolean();
    }

    internal static string? ReadOptionalString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new CodecException($"Property '{propertyName}' must be a string.");
        }

        return property.GetString();
    }

    internal static DateTimeOffset ReadRequiredTimestamp(JsonElement element, string propertyName)
    {
        var raw = ReadRequiredString(element, propertyName);
        return ReadRequiredTimestamp(raw, propertyName);
    }

    internal static DateTimeOffset ReadRequiredUtcTimestamp(JsonElement element, string propertyName)
    {
        var raw = ReadRequiredString(element, propertyName);
        return ReadRequiredTimestampWithAssumedOffset(raw, propertyName, TimeSpan.Zero);
    }

    internal static DateTimeOffset ReadRequiredJstTimestamp(JsonElement element, string propertyName)
    {
        var raw = ReadRequiredString(element, propertyName);
        return ReadRequiredTimestampWithAssumedOffset(raw, propertyName, JstOffset);
    }

    internal static DateTimeOffset? ReadOptionalUtcTimestamp(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new CodecException($"Property '{propertyName}' must be a timestamp string.");
        }

        var raw = property.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return ReadRequiredTimestampWithAssumedOffset(raw, propertyName, TimeSpan.Zero);
    }

    internal static DateTimeOffset? ReadOptionalJstTimestamp(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new CodecException($"Property '{propertyName}' must be a timestamp string.");
        }

        var raw = property.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return ReadRequiredTimestampWithAssumedOffset(raw, propertyName, JstOffset);
    }

    internal static decimal ReadDecimal(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Number || !element.TryGetDecimal(out var value))
        {
            throw new CodecException($"Property '{propertyName}' must be a decimal number.");
        }

        return value;
    }

    internal static DateTimeOffset ReadTimestamp(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.String)
        {
            throw new CodecException($"Property '{propertyName}' must be a timestamp.");
        }

        return ReadRequiredTimestamp(element.GetString(), propertyName);
    }

    private static DateTimeOffset ReadRequiredTimestamp(string? raw, string propertyName)
    {
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

    private static DateTimeOffset ReadRequiredTimestampWithAssumedOffset(string? raw, string propertyName, TimeSpan assumedOffset)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new CodecException($"Property '{propertyName}' must be a timestamp.");
        }

        if (HasExplicitOffset(raw))
        {
            if (DateTimeOffset.TryParse(
                raw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var explicitValue))
            {
                return explicitValue.ToOffset(TimeSpan.Zero);
            }

            throw new CodecException($"Property '{propertyName}' must be a timestamp.");
        }

        if (DateTime.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var localValue))
        {
            var unspecified = DateTime.SpecifyKind(localValue, DateTimeKind.Unspecified);
            return new DateTimeOffset(unspecified, assumedOffset).ToOffset(TimeSpan.Zero);
        }

        throw new CodecException($"Property '{propertyName}' must be a timestamp.");
    }

    private static bool HasExplicitOffset(string raw)
    {
        if (raw.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return raw.Length >= 6
            && (raw[^6] == '+' || raw[^6] == '-')
            && char.IsDigit(raw[^5])
            && char.IsDigit(raw[^4])
            && raw[^3] == ':'
            && char.IsDigit(raw[^2])
            && char.IsDigit(raw[^1]);
    }
}
