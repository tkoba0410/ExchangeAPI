using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(TransactionIdJsonConverter))]
public readonly record struct TransactionId
{
    public TransactionId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public override string ToString() => Value;

    public static TransactionId Empty { get; } = new(string.Empty);

    public static TransactionId Parse(string? value) =>
        new(value ?? string.Empty);

    public static TransactionId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("TransactionId value is required.", nameof(value));
        }

        return new TransactionId(value);
    }

    public static bool TryParse(string? value, out TransactionId transactionId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            transactionId = Empty;
            return false;
        }

        transactionId = new TransactionId(value);
        return true;
    }
}

internal sealed class TransactionIdJsonConverter : JsonConverter<TransactionId>
{
    public override TransactionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new TransactionId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for TransactionId.");
    }

    public override void Write(Utf8JsonWriter writer, TransactionId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
