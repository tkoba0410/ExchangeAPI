using System.Text.Json;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class TimestampJsonConverterTests
{
    [Fact]
    public void UtcConverter_Reads_NoOffset_Timestamp_As_Utc()
    {
        var value = JsonSerializer.Deserialize<UtcHolder>("""{"value":"2024-01-02T03:04:05.678"}""");

        Assert.NotNull(value);
        Assert.Equal(new DateTimeOffset(2024, 1, 2, 3, 4, 5, 678, TimeSpan.Zero), value!.Value);
    }

    [Fact]
    public void JstConverter_Reads_NoOffset_Timestamp_As_Jst_And_Normalizes_To_Utc()
    {
        var value = JsonSerializer.Deserialize<JstHolder>("""{"value":"2024-01-02T03:04:05.678"}""");

        Assert.NotNull(value);
        Assert.Equal(new DateTimeOffset(2024, 1, 1, 18, 4, 5, 678, TimeSpan.Zero), value!.Value);
    }

    [Fact]
    public void NullableUtcConverter_Writes_String_Token()
    {
        var json = JsonSerializer.Serialize(new NullableUtcHolder
        {
            Value = new DateTimeOffset(2024, 1, 2, 3, 4, 5, 678, TimeSpan.Zero),
        });

        using var document = JsonDocument.Parse(json);
        Assert.Equal(JsonValueKind.String, document.RootElement.GetProperty("value").ValueKind);
    }

    private sealed class UtcHolder
    {
        [JsonPropertyName("value")]
        [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
        public required DateTimeOffset Value { get; init; }
    }

    private sealed class JstHolder
    {
        [JsonPropertyName("value")]
        [JsonConverter(typeof(BitflyerJstTimestampJsonConverter))]
        public required DateTimeOffset Value { get; init; }
    }

    private sealed class NullableUtcHolder
    {
        [JsonPropertyName("value")]
        [JsonConverter(typeof(BitflyerNullableUtcTimestampJsonConverter))]
        public DateTimeOffset? Value { get; init; }
    }
}
