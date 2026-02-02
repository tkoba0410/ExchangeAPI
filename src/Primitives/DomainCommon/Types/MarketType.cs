using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

[JsonConverter(typeof(MarketTypeJsonConverter))]
public readonly record struct MarketType(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static MarketType Unknown { get; } = new("Unknown");

    public override string ToString() => Value ?? string.Empty;

    public static MarketType Parse(string? value)
    {
        return TryParse(value, out var type) ? type : Unknown;
    }

    public static MarketType ParseOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("MarketType value is required.", nameof(value));
        }

        return new MarketType(value);
    }

    public static bool TryParse(string? value, out MarketType type)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            type = Unknown;
            return false;
        }

        type = new MarketType(value);
        return true;
    }
}

internal sealed class MarketTypeJsonConverter : JsonConverter<MarketType>
{
    public override MarketType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
        return MarketType.ParseOrThrow(value);
    }

    public override void Write(Utf8JsonWriter writer, MarketType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
