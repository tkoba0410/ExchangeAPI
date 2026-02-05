using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(AcceptanceIdJsonConverter))]
public readonly record struct AcceptanceId
{
    public AcceptanceId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static AcceptanceId Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static AcceptanceId Parse(string? value) =>
        TryParse(value, out var id) ? id : Empty;

    public static AcceptanceId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("AcceptanceId value is required.", nameof(value));
        }

        return new AcceptanceId(value);
    }

    public static bool TryParse(string? value, out AcceptanceId id)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            id = Empty;
            return false;
        }

        id = new AcceptanceId(value);
        return true;
    }
}

internal sealed class AcceptanceIdJsonConverter : JsonConverter<AcceptanceId>
{
    public override AcceptanceId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new AcceptanceId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for AcceptanceId.");
    }

    public override void Write(Utf8JsonWriter writer, AcceptanceId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
