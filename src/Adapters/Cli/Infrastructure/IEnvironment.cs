namespace ExchangeApi.Adapters.Cli.Infrastructure;

public interface IEnvironment
{
    string? GetEnvironmentVariable(string name);
}
