namespace ExchangeApi.Adapters.McpServer.Tools;

public sealed class McpToolCallResult
{
    private McpToolCallResult(object structuredContent, bool isError)
    {
        StructuredContent = structuredContent;
        IsError = isError;
    }

    public object StructuredContent { get; }

    public bool IsError { get; }

    public static McpToolCallResult Success(object structuredContent)
    {
        return new McpToolCallResult(structuredContent, isError: false);
    }

    public static McpToolCallResult ToolError(object structuredContent)
    {
        return new McpToolCallResult(structuredContent, isError: true);
    }
}
