using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(ExecutionIdJsonConverter))]
public readonly record struct ExecutionId
{
    public ExecutionId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static ExecutionId Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static ExecutionId Parse(string? value) =>
        TryParse(value, out var id) ? id : Empty;

    public static ExecutionId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("ExecutionId value is required.", nameof(value));
        }

        return new ExecutionId(value);
    }

    public static bool TryParse(string? value, out ExecutionId id)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            id = Empty;
            return false;
        }

        id = new ExecutionId(value);
        return true;
    }
}

internal sealed class ExecutionIdJsonConverter : JsonConverter<ExecutionId>
{
    public override ExecutionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new ExecutionId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for ExecutionId.");
    }

    public override void Write(Utf8JsonWriter writer, ExecutionId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
