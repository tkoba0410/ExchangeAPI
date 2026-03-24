using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;

public sealed class GetBoardLevel
{
    [JsonPropertyName("price")]
    public required decimal Price { get; init; }

    [JsonPropertyName("size")]
    public required decimal Size { get; init; }
}
