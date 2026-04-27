using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

public sealed record BitflyerRealtimeBoardLevel
{
    [JsonPropertyName("price")]
    public required decimal Price { get; init; }

    [JsonPropertyName("size")]
    public required decimal Size { get; init; }
}
