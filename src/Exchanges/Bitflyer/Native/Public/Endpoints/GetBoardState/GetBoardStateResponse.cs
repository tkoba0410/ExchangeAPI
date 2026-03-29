using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;

public sealed class GetBoardStateResponse
{
    [JsonPropertyName("health")]
    public required string Health { get; init; }
    [JsonPropertyName("state")]
    public required string State { get; init; }
    [JsonPropertyName("data")]
    public GetBoardStateData? Data { get; init; }
}

public sealed class GetBoardStateData
{
    [JsonPropertyName("special_quotation")]
    public decimal? SpecialQuotation { get; init; }
}
