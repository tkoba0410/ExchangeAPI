namespace ExchangeApi.Adapters.McpServer.Schema;

public sealed class McpToolExecutionResult<TResponse>
{
    private McpToolExecutionResult(TResponse? response, McpToolError? error)
    {
        Response = response;
        Error = error;
    }

    public TResponse? Response { get; }

    public McpToolError? Error { get; }

    public bool IsSuccess => Error is null;

    public static McpToolExecutionResult<TResponse> Success(TResponse response)
    {
        return new McpToolExecutionResult<TResponse>(response, null);
    }

    public static McpToolExecutionResult<TResponse> Failure(McpToolError error)
    {
        return new McpToolExecutionResult<TResponse>(default, error);
    }
}
