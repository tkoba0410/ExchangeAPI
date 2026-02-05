using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(CursorJsonConverter))]
public readonly record struct Cursor
{
    public Cursor(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static Cursor Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static Cursor Parse(string? value) =>
        TryParse(value, out var cursor) ? cursor : Empty;

    public static Cursor ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Cursor value is required.", nameof(value));
        }

        return new Cursor(value);
    }

    public static bool TryParse(string? value, out Cursor cursor)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            cursor = Empty;
            return false;
        }

        cursor = new Cursor(value);
        return true;
    }
}

internal sealed class CursorJsonConverter : JsonConverter<Cursor>
{
    public override Cursor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new Cursor(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for Cursor.");
    }

    public override void Write(Utf8JsonWriter writer, Cursor value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
