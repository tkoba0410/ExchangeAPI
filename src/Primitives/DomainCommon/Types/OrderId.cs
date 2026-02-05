using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(OrderIdJsonConverter))]
public readonly record struct OrderId
{
    public OrderId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static OrderId Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static OrderId Parse(string? value) =>
        TryParse(value, out var id) ? id : Empty;

    public static OrderId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("OrderId value is required.", nameof(value));
        }

        return new OrderId(value);
    }

    public static bool TryParse(string? value, out OrderId id)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            id = Empty;
            return false;
        }

        id = new OrderId(value);
        return true;
    }
}

internal sealed class OrderIdJsonConverter : JsonConverter<OrderId>
{
    public override OrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new OrderId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for OrderId.");
    }

    public override void Write(Utf8JsonWriter writer, OrderId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
