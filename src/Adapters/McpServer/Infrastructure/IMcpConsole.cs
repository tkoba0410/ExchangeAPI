namespace ExchangeApi.Adapters.McpServer.Infrastructure;

public interface IMcpConsole
{
    void WriteOut(string value);

    void WriteOutLine(string value);

    void WriteError(string value);

    void WriteErrorLine(string value);
}
