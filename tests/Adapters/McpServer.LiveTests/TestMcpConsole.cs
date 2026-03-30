using System.Text;
using ExchangeApi.Adapters.McpServer.Infrastructure;

namespace ExchangeApi.Tests.Adapters.McpServer.LiveTests;

internal sealed class TestMcpConsole : IMcpConsole
{
    private readonly Queue<string> _stdin;
    private readonly StringBuilder _stdout = new();
    private readonly StringBuilder _stderr = new();

    public TestMcpConsole(params string[] inputLines)
    {
        _stdin = new Queue<string>(inputLines);
    }

    public string StdOut => _stdout.ToString();

    public string StdErr => _stderr.ToString();

    public Task<string?> ReadInLineAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return Task.FromResult(_stdin.Count > 0 ? _stdin.Dequeue() : null);
    }

    public void WriteOut(string value)
    {
        _stdout.Append(value);
    }

    public void WriteOutLine(string value)
    {
        _stdout.AppendLine(value);
    }

    public void WriteError(string value)
    {
        _stderr.Append(value);
    }

    public void WriteErrorLine(string value)
    {
        _stderr.AppendLine(value);
    }
}
