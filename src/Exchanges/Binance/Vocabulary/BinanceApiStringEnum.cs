using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Binance.Vocabulary;

[AttributeUsage(AttributeTargets.Field)]
public sealed class BinanceApiStringEnumValueAttribute : Attribute
{
    public BinanceApiStringEnumValueAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; }
}

public static class BinanceApiStringEnum<TEnum> where TEnum : struct, Enum
{
    private static readonly IReadOnlyDictionary<string, TEnum> StringToEnum = BuildStringToEnum();
    private static readonly IReadOnlyDictionary<TEnum, string> EnumToString = BuildEnumToString();

    public static bool IsDefined(TEnum value)
    {
        return EnumToString.ContainsKey(value);
    }

    public static bool TryParse(string? value, out TEnum result)
    {
        if (!string.IsNullOrWhiteSpace(value) && StringToEnum.TryGetValue(value, out result))
        {
            return true;
        }

        result = default;
        return false;
    }

    public static string Format(TEnum value)
    {
        if (EnumToString.TryGetValue(value, out var rawValue))
        {
            return rawValue;
        }

        throw new InvalidOperationException($"{typeof(TEnum).Name} value '{value}' is not mapped to an API string.");
    }

    private static IReadOnlyDictionary<string, TEnum> BuildStringToEnum()
    {
        return typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => new
            {
                Value = field.GetCustomAttribute<BinanceApiStringEnumValueAttribute>()?.Value
                    ?? throw new InvalidOperationException($"{typeof(TEnum).Name}.{field.Name} is missing BinanceApiStringEnumValueAttribute."),
                EnumValue = (TEnum)field.GetValue(null)!,
            })
            .ToDictionary(x => x.Value, x => x.EnumValue, StringComparer.Ordinal);
    }

    private static IReadOnlyDictionary<TEnum, string> BuildEnumToString()
    {
        return typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => new
            {
                EnumValue = (TEnum)field.GetValue(null)!,
                Value = field.GetCustomAttribute<BinanceApiStringEnumValueAttribute>()?.Value
                    ?? throw new InvalidOperationException($"{typeof(TEnum).Name}.{field.Name} is missing BinanceApiStringEnumValueAttribute."),
            })
            .ToDictionary(x => x.EnumValue, x => x.Value);
    }
}

public sealed class BinanceApiStringEnumJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for {typeof(TEnum).Name}.");
        }

        var raw = reader.GetString();
        if (BinanceApiStringEnum<TEnum>.TryParse(raw, out var value))
        {
            return value;
        }

        throw new JsonException($"Value '{raw}' is not valid for {typeof(TEnum).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(BinanceApiStringEnum<TEnum>.Format(value));
    }
}
