using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;

public sealed class GetBoardStateResponse
{
    [JsonPropertyName("health")]
    public required BitflyerHealthStatus Health { get; init; }
    [JsonPropertyName("state")]
    public required BitflyerTradingState State { get; init; }
    [JsonPropertyName("data")]
    public GetBoardStateData? Data { get; init; }
}

public sealed class GetBoardStateData
{
    [JsonPropertyName("special_quotation")]
    public decimal? SpecialQuotation { get; init; }
}
