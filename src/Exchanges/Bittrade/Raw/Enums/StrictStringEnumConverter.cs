using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public abstract class StrictStringEnumConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    private readonly IReadOnlyDictionary<string, T> _byValue;
    private readonly IReadOnlyDictionary<T, string> _byEnum;

    protected StrictStringEnumConverter()
    {
        var values = Enum.GetValues<T>();
        var byValue = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        var byEnum = new Dictionary<T, string>();
        foreach (var value in values)
        {
            var name = Enum.GetName(value) ?? value.ToString();
            var member = typeof(T).GetMember(name).FirstOrDefault();
            var enumMember = member?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .OfType<EnumMemberAttribute>()
                .FirstOrDefault();
            var wire = enumMember?.Value ?? name;
            byValue[wire] = value;
            byEnum[value] = wire;
        }

        _byValue = byValue;
        _byEnum = byEnum;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {typeof(T).Name}.");
        }

        var value = reader.GetString();
        if (value is null || !_byValue.TryGetValue(value, out var result))
        {
            throw new JsonException($"Unknown {typeof(T).Name} value: {value ?? "<null>"}.");
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (!_byEnum.TryGetValue(value, out var text))
        {
            throw new JsonException($"Unknown {typeof(T).Name} value: {value}.");
        }

        writer.WriteStringValue(text);
    }
}
