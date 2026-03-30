using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Infrastructure;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Tools;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class McpApplicationTests
{
    [Fact]
    public async Task NoArgs_StartsStdioLoopAndExitsSuccessfullyOnEof()
    {
        var console = new FakeMcpConsole();
        var app = new McpApplication(dispatcher: new FakeToolDispatcher(), console: console);

        var exitCode = await app.RunAsync([]);

        Assert.Equal(McpExitCode.Success, exitCode);
        Assert.Equal(string.Empty, console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task StdioLoop_RespondsToInitializeAndToolsList()
    {
        var console = new FakeMcpConsole(
            """{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}""",
            """{"jsonrpc":"2.0","method":"notifications/initialized"}""",
            """{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}""");
        var app = new McpApplication(dispatcher: new FakeToolDispatcher(), console: console);

        var exitCode = await app.RunAsync([]);

        Assert.Equal(McpExitCode.Success, exitCode);
        Assert.Equal(string.Empty, console.StdErr);

        var lines = GetOutputLines(console.StdOut);
        Assert.Equal(2, lines.Count);

        using var initialize = JsonDocument.Parse(lines[0]);
        Assert.Equal("2.0", initialize.RootElement.GetProperty("jsonrpc").GetString());
        Assert.Equal("2025-11-25", initialize.RootElement.GetProperty("result").GetProperty("protocolVersion").GetString());
        Assert.True(initialize.RootElement.GetProperty("result").GetProperty("capabilities").GetProperty("tools").TryGetProperty("listChanged", out var listChanged));
        Assert.False(listChanged.GetBoolean());

        using var toolsList = JsonDocument.Parse(lines[1]);
        var tools = toolsList.RootElement.GetProperty("result").GetProperty("tools");
        Assert.Equal(4, tools.GetArrayLength());
        Assert.Equal(["get_market_snapshot", "get_klines", "get_account_snapshot", "evaluate_order"], tools.EnumerateArray().Select(item => item.GetProperty("name").GetString()!).ToArray());
        Assert.Equal("object", tools[0].GetProperty("inputSchema").GetProperty("type").GetString());
        Assert.Equal("object", tools[0].GetProperty("outputSchema").GetProperty("type").GetString());
        Assert.True(tools[0].GetProperty("annotations").GetProperty("readOnlyHint").GetBoolean());
    }

    [Fact]
    public async Task StdioLoop_ToolsList_UsesDispatcherVisibleSurface()
    {
        var console = new FakeMcpConsole(
            """{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}""",
            """{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}""");
        var app = new McpApplication(
            dispatcher: new FakeToolDispatcher(ToolCatalog.PublicOnly),
            console: console);

        var exitCode = await app.RunAsync([]);

        Assert.Equal(McpExitCode.Success, exitCode);
        var lines = GetOutputLines(console.StdOut);
        Assert.Equal(2, lines.Count);

        using var toolsList = JsonDocument.Parse(lines[1]);
        var tools = toolsList.RootElement.GetProperty("result").GetProperty("tools");
        Assert.Equal(["get_market_snapshot", "get_klines"], tools.EnumerateArray().Select(item => item.GetProperty("name").GetString()!).ToArray());
    }

    [Fact]
    public async Task StdioLoop_ReturnsSuccessfulToolCallResult()
    {
        var console = new FakeMcpConsole(
            """{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}""",
            """{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"get_market_snapshot","arguments":{"symbol":"BTC_JPY"}}}""");
        var app = new McpApplication(dispatcher: new FakeToolDispatcher(), console: console);

        var exitCode = await app.RunAsync([]);

        Assert.Equal(McpExitCode.Success, exitCode);
        var lines = GetOutputLines(console.StdOut);
        Assert.Equal(2, lines.Count);

        using var toolCall = JsonDocument.Parse(lines[1]);
        var result = toolCall.RootElement.GetProperty("result");
        Assert.False(result.GetProperty("isError").GetBoolean());
        Assert.Equal("ok", result.GetProperty("structuredContent").GetProperty("message").GetString());
        Assert.Equal("""{"message":"ok"}""", result.GetProperty("content")[0].GetProperty("text").GetString());
    }

    [Fact]
    public async Task StdioLoop_ReturnsToolExecutionErrorAsIsErrorResult()
    {
        var console = new FakeMcpConsole(
            """{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}""",
            """{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"evaluate_order","arguments":{"symbol":"BTC_JPY"}}}""");
        var app = new McpApplication(dispatcher: new FakeToolDispatcher(), console: console);

        var exitCode = await app.RunAsync([]);

        Assert.Equal(McpExitCode.Success, exitCode);
        var lines = GetOutputLines(console.StdOut);
        Assert.Equal(2, lines.Count);

        using var toolCall = JsonDocument.Parse(lines[1]);
        var result = toolCall.RootElement.GetProperty("result");
        Assert.True(result.GetProperty("isError").GetBoolean());
        Assert.Equal("validation_error", result.GetProperty("structuredContent").GetProperty("errorCategory").GetString());
        Assert.Equal("invalid_request", result.GetProperty("structuredContent").GetProperty("errorCode").GetString());
    }

    [Fact]
    public async Task StdioLoop_ReturnsProtocolErrorForUnknownTool()
    {
        var console = new FakeMcpConsole(
            """{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}""",
            """{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"unknown_tool","arguments":{}}}""");
        var app = new McpApplication(dispatcher: new FakeToolDispatcher(), console: console);

        var exitCode = await app.RunAsync([]);

        Assert.Equal(McpExitCode.Success, exitCode);
        var lines = GetOutputLines(console.StdOut);
        Assert.Equal(2, lines.Count);

        using var error = JsonDocument.Parse(lines[1]);
        Assert.Equal(-32602, error.RootElement.GetProperty("error").GetProperty("code").GetInt32());
        Assert.Equal("Unknown tool: unknown_tool", error.RootElement.GetProperty("error").GetProperty("message").GetString());
    }

    [Fact]
    public async Task Help_PrintsPlannedToolNamesToStderr()
    {
        var console = new FakeMcpConsole();
        var app = new McpApplication(dispatcher: new FakeToolDispatcher(), console: console);

        var exitCode = await app.RunAsync(["--help"]);

        Assert.Equal(McpExitCode.Success, exitCode);
        Assert.Equal(string.Empty, console.StdOut);
        Assert.Contains("get_market_snapshot", console.StdErr);
        Assert.Contains("get_klines", console.StdErr);
        Assert.Contains("get_account_snapshot", console.StdErr);
        Assert.Contains("evaluate_order", console.StdErr);
    }

    [Fact]
    public async Task UnknownArgument_ReturnsArgumentError()
    {
        var console = new FakeMcpConsole();
        var app = new McpApplication(dispatcher: new FakeToolDispatcher(), console: console);

        var exitCode = await app.RunAsync(["--unknown"]);

        Assert.Equal(McpExitCode.ArgumentError, exitCode);
        Assert.Equal(string.Empty, console.StdOut);
        Assert.Contains("unknown argument: --unknown", console.StdErr);
    }

    private static IReadOnlyList<string> GetOutputLines(string stdout)
    {
        return stdout
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private sealed class FakeToolDispatcher : IMcpToolDispatcher
    {
        public FakeToolDispatcher(IReadOnlyList<McpToolDefinition>? tools = null)
        {
            Tools = tools ?? ToolCatalog.All;
        }

        public IReadOnlyList<McpToolDefinition> Tools { get; }

        public Task<McpToolCallResult> DispatchAsync(
            string name,
            JsonElement arguments,
            CancellationToken cancellationToken = default)
        {
            _ = arguments;
            _ = cancellationToken;

            return Task.FromResult(
                name switch
                {
                    "get_market_snapshot" => McpToolCallResult.Success(new { message = "ok" }),
                    "get_klines" => McpToolCallResult.Success(new { candles = 1 }),
                    "get_account_snapshot" => McpToolCallResult.Success(new { count = 1 }),
                    "evaluate_order" => McpToolCallResult.ToolError(
                        new McpToolError
                        {
                            ErrorCategory = "validation_error",
                            ErrorCode = "invalid_request",
                            Message = "Invalid request.",
                            Details = new Dictionary<string, string?>(),
                            Retryable = false,
                        }),
                    _ => throw new InvalidOperationException($"Unexpected tool name in fake dispatcher: {name}"),
                });
        }
    }
}
