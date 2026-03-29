using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;

public sealed class GetHealthResponse
{
    [JsonPropertyName("status")]
    public required string Status { get; init; }
}
