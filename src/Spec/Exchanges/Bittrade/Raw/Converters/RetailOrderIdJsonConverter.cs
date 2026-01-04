using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class RetailOrderIdJsonConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        IdJsonConverterHelpers.ReadStringOrNumber(ref reader);

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) =>
        throw new NotSupportedException(
            "Raw JsonConverter is read-only (Deserialize only). Serialize is not allowed in Raw layer.");
}
