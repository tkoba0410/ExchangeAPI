using ExchangeApi.Adapters.McpServer.Schema;

namespace ExchangeApi.Adapters.McpServer.Tools;

public sealed class McpToolCallResult
{
    private McpToolCallResult(object structuredContent, bool isError, McpToolCallMeta? meta)
    {
        StructuredContent = structuredContent;
        IsError = isError;
        Meta = meta;
    }

    public object StructuredContent { get; }

    public bool IsError { get; }

    public McpToolCallMeta? Meta { get; }

    public static McpToolCallResult Success(object structuredContent, McpToolCallMeta? meta = null)
    {
        return new McpToolCallResult(structuredContent, isError: false, meta);
    }

    public static McpToolCallResult ToolError(object structuredContent, McpToolCallMeta? meta = null)
    {
        return new McpToolCallResult(structuredContent, isError: true, meta);
    }
}
