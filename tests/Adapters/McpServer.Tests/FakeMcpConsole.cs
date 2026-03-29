using System.Text;
using ExchangeApi.Adapters.McpServer.Infrastructure;

namespace ExchangeApi.Adapters.McpServer.Tests;

internal sealed class FakeMcpConsole : IMcpConsole
{
    private readonly StringBuilder _stdout = new();
    private readonly StringBuilder _stderr = new();

    public string StdOut => _stdout.ToString();

    public string StdErr => _stderr.ToString();

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
