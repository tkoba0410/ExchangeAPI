using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class MatchResultIdJsonConverter : JsonConverter<RawMatchResultId>
{
    public override RawMatchResultId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(IdJsonConverterHelpers.ReadStringOrNumber(ref reader));

    public override void Write(Utf8JsonWriter writer, RawMatchResultId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
