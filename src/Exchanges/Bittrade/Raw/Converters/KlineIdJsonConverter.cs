using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class KlineIdJsonConverter : JsonConverter<KlineId>
{
    public override KlineId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(IdJsonConverterHelpers.ReadLong(ref reader));

    public override void Write(Utf8JsonWriter writer, KlineId value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.Value);
}
