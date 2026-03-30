namespace ExchangeApi.Adapters.McpServer.Infrastructure;

public sealed class SystemMcpConsole : IMcpConsole
{
    public Task<string?> ReadInLineAsync(CancellationToken cancellationToken)
    {
        return Console.In.ReadLineAsync(cancellationToken).AsTask();
    }

    public void WriteOut(string value)
    {
        Console.Out.Write(value);
    }

    public void WriteOutLine(string value)
    {
        Console.Out.WriteLine(value);
    }

    public void WriteError(string value)
    {
        Console.Error.Write(value);
    }

    public void WriteErrorLine(string value)
    {
        Console.Error.WriteLine(value);
    }
}
