using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;

public sealed class GetFundingRateRequest
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }
}
