using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

public sealed class BitflyerBoardEntry
{
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
}
