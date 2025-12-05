using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models;

public sealed class BitflyerBoardEntryRaw
{
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
}
