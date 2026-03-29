namespace ExchangeApi.Adapters.McpServer.Schema;

public sealed record McpToolDefinition(
    string Name,
    string Description,
    Type RequestType,
    Type ResponseType);
