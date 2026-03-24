using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;

public sealed class GetBoardResponse
{
    [JsonPropertyName("mid_price")]
    public required decimal MidPrice { get; init; }

    [JsonPropertyName("bids")]
    public required IReadOnlyList<GetBoardLevel> Bids { get; init; }

    [JsonPropertyName("asks")]
    public required IReadOnlyList<GetBoardLevel> Asks { get; init; }
}
