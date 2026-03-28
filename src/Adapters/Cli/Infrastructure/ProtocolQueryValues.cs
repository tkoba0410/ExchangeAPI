using System.Text.Json;

namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class ProtocolQueryValues
{
    private readonly IReadOnlyDictionary<string, JsonElement> _rawValues;
    private readonly IReadOnlyDictionary<string, object?> _typedValues;

    public ProtocolQueryValues(
        ProtocolQuerySchema schema,
        IReadOnlyDictionary<string, JsonElement> rawValues,
        IReadOnlyDictionary<string, object?> typedValues)
    {
        Schema = schema;
        _rawValues = rawValues;
        _typedValues = typedValues;
    }

    public ProtocolQuerySchema Schema { get; }

    public bool Contains(string name)
    {
        return _rawValues.ContainsKey(name);
    }

    public string? GetString(string name)
    {
        return _typedValues.TryGetValue(name, out var value) ? (string?)value : null;
    }

    public int? GetInt(string name)
    {
        return _typedValues.TryGetValue(name, out var value) ? (int?)value : null;
    }

    public long? GetLong(string name)
    {
        return _typedValues.TryGetValue(name, out var value) ? (long?)value : null;
    }

    public string Describe()
    {
        return Schema.Describe(this);
    }
}
