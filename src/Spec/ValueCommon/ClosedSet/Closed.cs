using System;

namespace ExchangeApi.Spec.ValueCommon.ClosedSet;

public readonly record struct Closed<T>(bool IsKnown, T Known, string? Unknown)
{
    public static Closed<T> KnownValue(T value) => new(true, value, null);
    public static Closed<T> UnknownValue(string raw) => new(false, default!, raw);

    public T RequireKnown(string fieldName)
        => IsKnown ? Known : throw new InvalidOperationException($"Unknown value for {fieldName}: '{Unknown}'.");

    public override string ToString() => IsKnown ? Known?.ToString() ?? string.Empty : Unknown ?? string.Empty;
}
