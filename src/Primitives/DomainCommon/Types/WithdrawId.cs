using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(WithdrawIdJsonConverter))]
public readonly record struct WithdrawId
{
    public WithdrawId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static WithdrawId Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static WithdrawId Parse(string? value) =>
        TryParse(value, out var withdrawId) ? withdrawId : Empty;

    public static WithdrawId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("WithdrawId value is required.", nameof(value));
        }

        return new WithdrawId(value);
    }

    public static bool TryParse(string? value, out WithdrawId withdrawId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            withdrawId = Empty;
            return false;
        }

        withdrawId = new WithdrawId(value);
        return true;
    }
}

internal sealed class WithdrawIdJsonConverter : JsonConverter<WithdrawId>
{
    public override WithdrawId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new WithdrawId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for WithdrawId.");
    }

    public override void Write(Utf8JsonWriter writer, WithdrawId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}
