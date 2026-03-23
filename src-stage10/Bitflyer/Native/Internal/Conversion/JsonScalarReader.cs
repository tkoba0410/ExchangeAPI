using System.Globalization;
using System.Text.Json;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Errors;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;

internal static class JsonScalarReader
{
    public static bool TryReadString(
        JsonElement element,
        string propertyName,
        out string? value,
        out CallError? error)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            value = null;
            error = null;
            return true;
        }

        if (property.ValueKind == JsonValueKind.String)
        {
            value = property.GetString();
            error = null;
            return true;
        }

        value = null;
        error = BitflyerErrorFactory.Mapping($"Property '{propertyName}' must be a string.");
        return false;
    }

    public static bool TryReadDecimal(
        JsonElement element,
        string propertyName,
        out decimal? value,
        out CallError? error)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            value = null;
            error = null;
            return true;
        }

        if (property.ValueKind == JsonValueKind.Number && property.TryGetDecimal(out var decimalValue))
        {
            value = decimalValue;
            error = null;
            return true;
        }

        if (property.ValueKind == JsonValueKind.String &&
            decimal.TryParse(
                property.GetString(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out decimalValue))
        {
            value = decimalValue;
            error = null;
            return true;
        }

        value = null;
        error = BitflyerErrorFactory.Mapping($"Property '{propertyName}' must be a decimal number.");
        return false;
    }

    public static bool TryReadInt64(
        JsonElement element,
        string propertyName,
        out long? value,
        out CallError? error)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            value = null;
            error = null;
            return true;
        }

        if (property.ValueKind == JsonValueKind.Number && property.TryGetInt64(out var int64Value))
        {
            value = int64Value;
            error = null;
            return true;
        }

        if (property.ValueKind == JsonValueKind.String &&
            long.TryParse(
                property.GetString(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int64Value))
        {
            value = int64Value;
            error = null;
            return true;
        }

        value = null;
        error = BitflyerErrorFactory.Mapping($"Property '{propertyName}' must be an integer.");
        return false;
    }

    public static bool TryReadDateTimeOffset(
        JsonElement element,
        string propertyName,
        out DateTimeOffset? value,
        out CallError? error)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            value = null;
            error = null;
            return true;
        }

        if (property.ValueKind == JsonValueKind.String &&
            DateTimeOffset.TryParse(
                property.GetString(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = null;
        error = BitflyerErrorFactory.Mapping($"Property '{propertyName}' must be an ISO-8601 timestamp.");
        return false;
    }
}
