using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Schema;

namespace ExchangeApi.Adapters.McpServer.Tools;

public interface IMcpToolDispatcher
{
    IReadOnlyList<McpToolDefinition> Tools { get; }

    Task<McpToolCallResult> DispatchAsync(
        string name,
        JsonElement arguments,
        CancellationToken cancellationToken = default);
}
