using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

public sealed class Board
{
    [JsonPropertyName("mid_price")] public decimal MidPrice { get; init; }
    [JsonPropertyName("bids")] public IReadOnlyList<BoardEntry> Bids { get; init; } = [];
    [JsonPropertyName("asks")] public IReadOnlyList<BoardEntry> Asks { get; init; } = [];
}
