using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Spec.JsonCommon.Converters;

public sealed class StringOrNumberToStringConverter : ReadOnlyJsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        IdJsonConverterHelpers.ReadStringOrNumber(ref reader);
}
