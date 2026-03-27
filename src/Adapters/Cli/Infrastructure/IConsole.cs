namespace ExchangeApi.Adapters.Cli.Infrastructure;

public interface IConsole
{
    bool IsInputRedirected { get; }
    bool IsErrorRedirected { get; }
    Task<string> ReadStandardInputToEndAsync(CancellationToken cancellationToken);
    Task<string?> ReadLineAsync(CancellationToken cancellationToken);
    void WriteOut(string value);
    void WriteOutLine(string value);
    void WriteError(string value);
    void WriteErrorLine(string value);
}
