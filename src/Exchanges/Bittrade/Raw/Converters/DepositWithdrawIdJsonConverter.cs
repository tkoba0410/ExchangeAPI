using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class DepositWithdrawIdJsonConverter : JsonConverter<RawDepositWithdrawId>
{
    public override RawDepositWithdrawId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(IdJsonConverterHelpers.ReadStringOrNumber(ref reader));

    public override void Write(Utf8JsonWriter writer, RawDepositWithdrawId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
