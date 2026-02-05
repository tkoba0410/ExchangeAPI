using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(AccountIdJsonConverter))]
public readonly record struct AccountId
{
    public AccountId(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static AccountId Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;

    public static AccountId Parse(string? value) =>
        TryParse(value, out var accountId) ? accountId : Empty;

    public static AccountId ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("AccountId value is required.", nameof(value));
        }

        return new AccountId(value);
    }

    public static bool TryParse(string? value, out AccountId accountId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            accountId = Empty;
            return false;
        }

        accountId = new AccountId(value);
        return true;
    }
}

internal sealed class AccountIdJsonConverter : JsonConverter<AccountId>
{
    public override AccountId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new AccountId(reader.GetString() ?? string.Empty);
        }

        throw new JsonException("Expected string token for AccountId.");
    }

    public override void Write(Utf8JsonWriter writer, AccountId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}
