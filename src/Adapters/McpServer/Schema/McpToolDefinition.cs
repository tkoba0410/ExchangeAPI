using System.Text.Json;

namespace ExchangeApi.Adapters.McpServer.Schema;

public sealed record McpToolDefinition(
    string Name,
    string Description,
    Type RequestType,
    Type ResponseType,
    string InputSchemaJson,
    string? OutputSchemaJson,
    bool ReadOnlyHint,
    bool RequiresCredentials);
