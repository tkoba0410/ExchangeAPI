using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;

public sealed class GetHealthResponse
{
    [JsonPropertyName("status")]
    public required BitflyerHealthStatus Status { get; init; }
}
