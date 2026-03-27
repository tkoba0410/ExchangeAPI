using System.Text;
using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Tests;

internal sealed class FakeConsole : IConsole
{
    private readonly Queue<string?> _lines = new();
    private readonly StringBuilder _stdout = new();
    private readonly StringBuilder _stderr = new();

    public bool IsInputRedirected { get; set; }
    public bool IsErrorRedirected { get; set; }

    public string StdOut => _stdout.ToString();
    public string StdErr => _stderr.ToString();

    public void EnqueueInputLine(string? line)
    {
        _lines.Enqueue(line);
    }

    public Task<string> ReadStandardInputToEndAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_lines.Count == 0 ? null : _lines.Dequeue());
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
