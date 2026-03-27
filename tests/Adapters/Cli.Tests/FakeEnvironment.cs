using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Tests;

internal sealed class FakeEnvironment : IEnvironment
{
    private readonly IReadOnlyDictionary<string, string?> _values;

    public FakeEnvironment(IReadOnlyDictionary<string, string?>? values = null)
    {
        _values = values ?? new Dictionary<string, string?>();
    }

    public string? GetEnvironmentVariable(string name)
    {
        return _values.TryGetValue(name, out var value) ? value : null;
    }
}
