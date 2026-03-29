using ExchangeApi.Adapters.McpServer.Infrastructure;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class McpApplicationTests
{
    [Fact]
    public async Task NoArgs_WritesStartupPlaceholderToStderr()
    {
        var console = new FakeMcpConsole();
        var app = new McpApplication(console: console);

        var exitCode = await app.RunAsync([]);

        Assert.Equal(McpExitCode.NotImplemented, exitCode);
        Assert.Equal(string.Empty, console.StdOut);
        Assert.Contains("MCP stdio transport is not implemented yet.", console.StdErr);
    }

    [Fact]
    public async Task Help_PrintsPlannedToolNamesToStderr()
    {
        var console = new FakeMcpConsole();
        var app = new McpApplication(console: console);

        var exitCode = await app.RunAsync(["--help"]);

        Assert.Equal(McpExitCode.Success, exitCode);
        Assert.Equal(string.Empty, console.StdOut);
        Assert.Contains("get_market_snapshot", console.StdErr);
        Assert.Contains("get_account_snapshot", console.StdErr);
        Assert.Contains("evaluate_order", console.StdErr);
    }

    [Fact]
    public async Task UnknownArgument_ReturnsArgumentError()
    {
        var console = new FakeMcpConsole();
        var app = new McpApplication(console: console);

        var exitCode = await app.RunAsync(["--unknown"]);

        Assert.Equal(McpExitCode.ArgumentError, exitCode);
        Assert.Equal(string.Empty, console.StdOut);
        Assert.Contains("unknown argument: --unknown", console.StdErr);
    }
}
