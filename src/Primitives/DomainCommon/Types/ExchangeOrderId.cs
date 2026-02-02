using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(ExchangeOrderIdJsonConverter))]
public readonly record struct ExchangeOrderId
{
    public ExchangeOrderId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static ExchangeOrderId Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static ExchangeOrderId Parse(string? value) =>
        TryParse(value, out var id) ? id : Empty;

    public static ExchangeOrderId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("ExchangeOrderId value is required.", nameof(value));
        }

        return new ExchangeOrderId(value);
    }

    public static bool TryParse(string? value, out ExchangeOrderId id)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            id = Empty;
            return false;
        }

        id = new ExchangeOrderId(value);
        return true;
    }
}

internal sealed class ExchangeOrderIdJsonConverter : JsonConverter<ExchangeOrderId>
{
    public override ExchangeOrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new ExchangeOrderId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for ExchangeOrderId.");
    }

    public override void Write(Utf8JsonWriter writer, ExchangeOrderId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
