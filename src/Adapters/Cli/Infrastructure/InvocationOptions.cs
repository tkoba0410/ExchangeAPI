namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class InvocationOptions
{
    private readonly IReadOnlyDictionary<string, string?> _options;

    public InvocationOptions(IReadOnlyDictionary<string, string?> options)
    {
        _options = options;
    }

    public bool HasFlag(string name)
    {
        return _options.TryGetValue(name, out var value) && value is null;
    }

    public bool HasValue(string name)
    {
        return _options.TryGetValue(name, out var value) && value is not null;
    }

    public string? GetValue(string name)
    {
        return _options.TryGetValue(name, out var value) ? value : null;
    }

    public bool Contains(string name)
    {
        return _options.ContainsKey(name);
    }

    public IReadOnlyCollection<string> Names => _options.Keys.ToArray();
}
