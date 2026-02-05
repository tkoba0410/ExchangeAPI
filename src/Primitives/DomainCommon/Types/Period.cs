using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(PeriodJsonConverter))]
public readonly record struct Period
{
    public Period(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static Period Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static Period Parse(string? value) =>
        TryParse(value, out var period) ? period : Empty;

    public static Period ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Period value is required.", nameof(value));
        }

        return new Period(value);
    }

    public static bool TryParse(string? value, out Period period)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            period = Empty;
            return false;
        }

        period = new Period(value);
        return true;
    }
}

internal sealed class PeriodJsonConverter : JsonConverter<Period>
{
    public override Period Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new Period(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for Period.");
    }

    public override void Write(Utf8JsonWriter writer, Period value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
