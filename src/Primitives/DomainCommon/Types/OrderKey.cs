using System;
using System.Text.Json.Serialization;

namespace ExchangeApi.Primitives.DomainCommon.Types;

public readonly record struct OrderKey
{
    [JsonConstructor]
    public OrderKey(OrderIdKind kind, string value) : this()
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("OrderKey value is required.", nameof(value));
        }

        Kind = kind;
        Value = value;
    }

    public OrderIdKind Kind { get; }
    public string Value { get; }

    public override string ToString() => $"{Kind}:{Value}";
}
