using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet;

public sealed class BoardEntry
{
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
}
