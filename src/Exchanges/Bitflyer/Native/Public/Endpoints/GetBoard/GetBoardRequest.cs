using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;

public sealed class GetBoardRequest
{
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; init; }
}
