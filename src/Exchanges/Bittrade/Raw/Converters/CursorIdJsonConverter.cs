using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class CursorIdJsonConverter : JsonConverter<CursorId>
{
    public override CursorId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(IdJsonConverterHelpers.ReadLong(ref reader));

    public override void Write(Utf8JsonWriter writer, CursorId value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.Value);
}
