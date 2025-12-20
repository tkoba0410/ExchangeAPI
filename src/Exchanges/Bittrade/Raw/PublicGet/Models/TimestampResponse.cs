using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record TimestampResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long Data);
