namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class SystemConsole : IConsole
{
    public bool IsInputRedirected => Console.IsInputRedirected;
    public bool IsErrorRedirected => Console.IsErrorRedirected;

    public Task<string> ReadStandardInputToEndAsync(CancellationToken cancellationToken)
    {
        return Console.In.ReadToEndAsync(cancellationToken);
    }

    public Task<string?> ReadLineAsync(CancellationToken cancellationToken)
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
