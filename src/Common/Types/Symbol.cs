using System.Text.Json.Serialization;

namespace ExchangeApi.Common.Types;

[JsonConverter(typeof(SymbolJsonConverter))]
public readonly record struct Symbol(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static Symbol Empty { get; } = new(string.Empty);

    public override string ToString() => Value ?? string.Empty;
}
