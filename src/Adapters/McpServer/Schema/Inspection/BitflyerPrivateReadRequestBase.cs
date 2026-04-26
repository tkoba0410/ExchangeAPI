using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public abstract class BitflyerPrivateReadRequestBase
{
    [JsonPropertyName("venue")]
    public string Venue { get; init; } = "bitflyer";

    [JsonPropertyName("accountContext")]
    public string AccountContext { get; init; } = "default";
}
