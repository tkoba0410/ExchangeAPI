using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;

public sealed class GetBoardStateRequest
{
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; init; }
}
