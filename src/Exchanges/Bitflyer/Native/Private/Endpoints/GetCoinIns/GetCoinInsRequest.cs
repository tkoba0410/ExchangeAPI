using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;

public sealed class GetCoinInsRequest
{
    [JsonPropertyName("count")]
    public int? Count { get; init; }
    [JsonPropertyName("before")]
    public long? Before { get; init; }
    [JsonPropertyName("after")]
    public long? After { get; init; }
}
