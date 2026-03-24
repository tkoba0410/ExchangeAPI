using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;

public sealed class GetPositionsRequest
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }
}
