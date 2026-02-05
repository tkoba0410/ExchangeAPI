using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.JsonCommon.Converters;

public abstract class ReadOnlyJsonConverter<T> : JsonConverter<T>
{
    public sealed override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => throw new NotSupportedException(
            "Raw JsonConverter is read-only (Deserialize only). Serialize is not allowed in Raw layer.");
}
