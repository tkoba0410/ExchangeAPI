namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class ProcessEnvironment : IEnvironment
{
    public bool AllowDefaultCredentialProfileDiscovery => true;

    public string? GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name);
    }
}
