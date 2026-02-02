using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(FreeTextJsonConverter))]
public readonly record struct FreeText
{
    public FreeText(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static FreeText Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static FreeText Parse(string? value) =>
        TryParse(value, out var text) ? text : Empty;

    public static FreeText ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("FreeText value is required.", nameof(value));
        }

        return new FreeText(value);
    }

    public static bool TryParse(string? value, out FreeText text)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            text = Empty;
            return false;
        }

        text = new FreeText(value);
        return true;
    }
}

internal sealed class FreeTextJsonConverter : JsonConverter<FreeText>
{
    public override FreeText Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new FreeText(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for FreeText.");
    }

    public override void Write(Utf8JsonWriter writer, FreeText value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
