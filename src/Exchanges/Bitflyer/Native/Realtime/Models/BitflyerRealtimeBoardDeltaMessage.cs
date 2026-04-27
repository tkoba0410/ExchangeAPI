using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

public sealed record BitflyerRealtimeBoardDeltaMessage : IProductRealtimeMessage
{
    public required string Channel { get; init; }

    [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
    public required DateTimeOffset ReceivedAt { get; init; }
    public required string ProductCode { get; init; }

    [JsonPropertyName("mid_price")]
    public required decimal MidPrice { get; init; }

    [JsonPropertyName("bids")]
    public required IReadOnlyList<BitflyerRealtimeBoardLevel> Bids { get; init; }

    [JsonPropertyName("asks")]
    public required IReadOnlyList<BitflyerRealtimeBoardLevel> Asks { get; init; }
}
