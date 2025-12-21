using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed class BittradeRetailOrderTypeJsonConverter : JsonConverter<BittradeRetailOrderType>
{
    public override BittradeRetailOrderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException($"Expected number for {nameof(BittradeRetailOrderType)}.");
        }

        var value = reader.GetInt32();
        if (!Enum.IsDefined(typeof(BittradeRetailOrderType), value))
        {
            throw new JsonException($"Unknown {nameof(BittradeRetailOrderType)} value: {value}.");
        }

        return (BittradeRetailOrderType)value;
    }

    public override void Write(Utf8JsonWriter writer, BittradeRetailOrderType value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}
