using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Bitflyer.Models;

public sealed class BitflyerBoardRaw
{
    [JsonPropertyName("mid_price")] public decimal MidPrice { get; init; }
    [JsonPropertyName("bids")] public IReadOnlyList<BitflyerBoardEntryRaw> Bids { get; init; } = [];
    [JsonPropertyName("asks")] public IReadOnlyList<BitflyerBoardEntryRaw> Asks { get; init; } = [];
}
