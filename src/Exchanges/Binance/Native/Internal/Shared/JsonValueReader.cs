using System.Globalization;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Binance.Native.Internal.Shared;

internal static class JsonValueReader
{
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

    internal static void EnsureArrayLength(JsonElement arrayElement, int expectedLength, string context)
    {
        if (arrayElement.ValueKind != JsonValueKind.Array)
        {
            throw new CodecException($"{context} must be an array.");
        }

        if (arrayElement.GetArrayLength() != expectedLength)
        {
            throw new CodecException($"{context} must contain exactly {expectedLength} items.");
        }
    }

    internal static long ReadRequiredInt64At(JsonElement arrayElement, int index, string context)
    {
        var value = ReadRequiredElementAt(arrayElement, index, context);
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt64(out var result))
        {
            throw new CodecException($"{context}[{index}] must be an integer number.");
        }

        return result;
    }

    internal static int ReadRequiredInt32At(JsonElement arrayElement, int index, string context)
    {
        var value = ReadRequiredElementAt(arrayElement, index, context);
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out var result))
        {
            throw new CodecException($"{context}[{index}] must be an integer number.");
        }

        return result;
    }

    internal static decimal ReadRequiredDecimalStringAt(JsonElement arrayElement, int index, string context)
    {
        var value = ReadRequiredElementAt(arrayElement, index, context);
        if (value.ValueKind != JsonValueKind.String)
        {
            throw new CodecException($"{context}[{index}] must be a string.");
        }

        var text = value.GetString();
        if (string.IsNullOrWhiteSpace(text) ||
            !decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            throw new CodecException($"{context}[{index}] must be a decimal string.");
        }

        return result;
    }

    private static JsonElement ReadRequiredElementAt(JsonElement arrayElement, int index, string context)
    {
        if (arrayElement.ValueKind != JsonValueKind.Array)
        {
            throw new CodecException($"{context} must be an array.");
        }

        if (index < 0 || index >= arrayElement.GetArrayLength())
        {
            throw new CodecException($"{context}[{index}] is missing.");
        }

        return arrayElement[index];
    }
}
