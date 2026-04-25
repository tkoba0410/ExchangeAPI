namespace ExchangeApi.Adapters.Cli.Infrastructure;

public interface IEnvironment
{
    bool AllowDefaultCredentialProfileDiscovery => false;

    string? GetEnvironmentVariable(string name);
}
