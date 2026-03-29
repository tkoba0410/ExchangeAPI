using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema;

public sealed class McpToolError
{
    [JsonPropertyName("errorCategory")]
    public required string ErrorCategory { get; init; }

    [JsonPropertyName("errorCode")]
    public required string ErrorCode { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("details")]
    public required IReadOnlyDictionary<string, string?> Details { get; init; }

    [JsonPropertyName("retryable")]
    public required bool Retryable { get; init; }
}
