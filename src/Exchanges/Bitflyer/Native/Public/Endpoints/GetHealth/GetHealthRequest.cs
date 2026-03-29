using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;

public sealed class GetHealthRequest
{
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; init; }
}
