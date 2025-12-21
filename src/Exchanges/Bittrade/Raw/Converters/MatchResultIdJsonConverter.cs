using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class MatchResultIdJsonConverter : JsonConverter<MatchResultId>
{
    public override MatchResultId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(IdJsonConverterHelpers.ReadStringOrNumber(ref reader));

    public override void Write(Utf8JsonWriter writer, MatchResultId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
