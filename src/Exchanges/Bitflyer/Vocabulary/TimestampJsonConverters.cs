using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Vocabulary;

public static class BitflyerTimestampJson
{
    private static readonly TimeSpan JstOffset = TimeSpan.FromHours(9);

    public static bool TryParseRaw(string? raw, out DateTimeOffset value)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            value = default;
            return false;
        }

        return DateTimeOffset.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out value);
    }

    public static bool TryParseUtc(string? raw, out DateTimeOffset value)
    {
        return TryParseWithAssumedOffset(raw, TimeSpan.Zero, out value);
    }

    public static bool TryParseJst(string? raw, out DateTimeOffset value)
    {
        return TryParseWithAssumedOffset(raw, JstOffset, out value);
    }

    private static bool TryParseWithAssumedOffset(string? raw, TimeSpan assumedOffset, out DateTimeOffset value)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            value = default;
            return false;
        }

        if (HasExplicitOffset(raw))
        {
            if (DateTimeOffset.TryParse(
                raw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var explicitValue))
            {
                value = explicitValue.ToOffset(TimeSpan.Zero);
                return true;
            }

            value = default;
            return false;
        }

        if (DateTime.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var localValue))
        {
            var unspecified = DateTime.SpecifyKind(localValue, DateTimeKind.Unspecified);
            value = new DateTimeOffset(unspecified, assumedOffset).ToOffset(TimeSpan.Zero);
            return true;
        }

        value = default;
        return false;
    }

    private static bool HasExplicitOffset(string raw)
    {
        if (raw.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return raw.Length >= 6
            && (raw[^6] == '+' || raw[^6] == '-')
            && char.IsDigit(raw[^5])
            && char.IsDigit(raw[^4])
            && raw[^3] == ':'
            && char.IsDigit(raw[^2])
            && char.IsDigit(raw[^1]);
    }
}

public sealed class BitflyerUtcTimestampJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for {typeof(DateTimeOffset).Name}.");
        }

        var raw = reader.GetString();
        if (BitflyerTimestampJson.TryParseUtc(raw, out var value))
        {
            return value;
        }

        throw new JsonException($"Value '{raw}' is not a valid UTC-assumed bitFlyer timestamp.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

public sealed class BitflyerNullableUtcTimestampJsonConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for {typeof(DateTimeOffset?).Name}.");
        }

        var raw = reader.GetString();
        if (BitflyerTimestampJson.TryParseUtc(raw, out var value))
        {
            return value;
        }

        throw new JsonException($"Value '{raw}' is not a valid UTC-assumed bitFlyer timestamp.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value);
    }
}

public sealed class BitflyerJstTimestampJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for {typeof(DateTimeOffset).Name}.");
        }

        var raw = reader.GetString();
        if (BitflyerTimestampJson.TryParseJst(raw, out var value))
        {
            return value;
        }

        throw new JsonException($"Value '{raw}' is not a valid JST-assumed bitFlyer timestamp.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

public sealed class BitflyerNullableJstTimestampJsonConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for {typeof(DateTimeOffset?).Name}.");
        }

        var raw = reader.GetString();
        if (BitflyerTimestampJson.TryParseJst(raw, out var value))
        {
            return value;
        }

        throw new JsonException($"Value '{raw}' is not a valid JST-assumed bitFlyer timestamp.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value);
    }
}
