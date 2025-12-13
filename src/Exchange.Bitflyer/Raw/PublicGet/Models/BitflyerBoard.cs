using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

public sealed class BitflyerBoard
{
    [JsonPropertyName("mid_price")] public decimal MidPrice { get; init; }
    [JsonPropertyName("bids")] public IReadOnlyList<BitflyerBoardEntry> Bids { get; init; } = [];
    [JsonPropertyName("asks")] public IReadOnlyList<BitflyerBoardEntry> Asks { get; init; } = [];
}
