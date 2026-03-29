using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Tools;

namespace ExchangeApi.Adapters.McpServer.Infrastructure;

public sealed class McpApplication
{
    private readonly IMcpConsole _console;
    private readonly IReadOnlyList<McpToolDefinition> _tools;

    public McpApplication(
        IReadOnlyList<McpToolDefinition>? tools = null,
        IMcpConsole? console = null)
    {
        _tools = tools ?? ToolCatalog.All;
        _console = console ?? new SystemMcpConsole();
    }

    public static Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        return new McpApplication().RunAsync(args, cancellationToken);
    }

    public Task<int> RunAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;

        try
        {
            return Task.FromResult(RunInternal(args));
        }
        catch (Exception ex)
        {
            _console.WriteErrorLine($"unexpected internal error: {ex.Message}");
            return Task.FromResult(McpExitCode.UnexpectedInternalError);
        }
    }

    private int RunInternal(IReadOnlyList<string> args)
    {
        if (args.Count == 0)
        {
            WriteStartupPlaceholder();
            return McpExitCode.NotImplemented;
        }

        if (args.Count == 1 && (args[0] == "--help" || args[0] == "-h"))
        {
            WriteHelp();
            return McpExitCode.Success;
        }

        _console.WriteErrorLine($"unknown argument: {string.Join(" ", args)}");
        _console.WriteErrorLine("Use --help to inspect the current scaffold.");
        return McpExitCode.ArgumentError;
    }

    private void WriteStartupPlaceholder()
    {
        _console.WriteErrorLine("MCP stdio transport is not implemented yet.");
        _console.WriteErrorLine("Use --help to inspect the planned tool surface.");
    }

    private void WriteHelp()
    {
        _console.WriteErrorLine("exchangeapi-mcp");
        _console.WriteErrorLine("Current scaffold for the ExchangeAPI MCP server.");
        _console.WriteErrorLine("Planned tools:");

        foreach (var tool in _tools)
        {
            _console.WriteErrorLine($"- {tool.Name}: {tool.Description}");
        }

        _console.WriteErrorLine("No stdio transport is implemented yet.");
    }
}
