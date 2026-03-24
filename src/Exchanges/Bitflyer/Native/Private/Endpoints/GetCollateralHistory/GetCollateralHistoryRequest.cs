using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;

public sealed class GetCollateralHistoryRequest
{
    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonPropertyName("before")]
    public long? Before { get; init; }

    [JsonPropertyName("after")]
    public long? After { get; init; }
}
