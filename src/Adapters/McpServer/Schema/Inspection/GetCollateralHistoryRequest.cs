using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public sealed class GetCollateralHistoryRequest : BitflyerPrivateReadRequestBase
{
    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonPropertyName("before")]
    public long? Before { get; init; }

    [JsonPropertyName("after")]
    public long? After { get; init; }
}
