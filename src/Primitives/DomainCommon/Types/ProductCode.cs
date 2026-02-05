using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(ProductCodeJsonConverter))]
public readonly record struct ProductCode(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static ProductCode Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static ProductCode Parse(string? value) =>
        TryParse(value, out var code) ? code : Empty;

    public static ProductCode ParseNormalized(string? value) =>
        TryParseNormalized(value, out var code) ? code : Empty;

    public static ProductCode ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("ProductCode value is required.", nameof(value));
        }

        return new ProductCode(value);
    }

    public static ProductCode ParseOrThrowNormalized(string? value)
    {
        if (!TryParseNormalized(value, out var code))
        {
            throw new ArgumentException("ProductCode value is required.", nameof(value));
        }

        return code;
    }

    public static bool TryParse(string? value, out ProductCode code)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            code = Empty;
            return false;
        }

        code = new ProductCode(value);
        return true;
    }

    public static bool TryParseNormalized(string? value, out ProductCode code)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            code = Empty;
            return false;
        }

        var normalized = Normalize(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            code = Empty;
            return false;
        }

        code = new ProductCode(normalized);
        return true;
    }

    private static string Normalize(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length == 0) return string.Empty;

        return trimmed
            .Replace('/', '_')
            .Replace('-', '_')
            .ToUpperInvariant();
    }
}

internal sealed class ProductCodeJsonConverter : JsonConverter<ProductCode>
{
    public override ProductCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
        return ProductCode.ParseOrThrowNormalized(value);
    }

    public override void Write(Utf8JsonWriter writer, ProductCode value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
