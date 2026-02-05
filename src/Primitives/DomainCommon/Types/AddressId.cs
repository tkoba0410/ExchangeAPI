using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(AddressIdJsonConverter))]
public readonly record struct AddressId
{
    public AddressId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static AddressId Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static AddressId Parse(string? value) =>
        TryParse(value, out var addressId) ? addressId : Empty;

    public static AddressId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("AddressId value is required.", nameof(value));
        }

        return new AddressId(value);
    }

    public static bool TryParse(string? value, out AddressId addressId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            addressId = Empty;
            return false;
        }

        addressId = new AddressId(value);
        return true;
    }
}

internal sealed class AddressIdJsonConverter : JsonConverter<AddressId>
{
    public override AddressId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new AddressId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for AddressId.");
    }

    public override void Write(Utf8JsonWriter writer, AddressId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}
