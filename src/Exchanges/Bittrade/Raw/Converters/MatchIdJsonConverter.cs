using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class MatchIdJsonConverter : JsonConverter<MatchId>
{
    public override MatchId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(IdJsonConverterHelpers.ReadStringOrNumber(ref reader));

    public override void Write(Utf8JsonWriter writer, MatchId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
