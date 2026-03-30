using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema;

public sealed class McpToolCallMeta
{
    [JsonPropertyName("schemaVersion")]
    public required string SchemaVersion { get; init; }

    [JsonPropertyName("dataVersion")]
    public required string DataVersion { get; init; }

    [JsonPropertyName("degraded")]
    public required bool Degraded { get; init; }
}
