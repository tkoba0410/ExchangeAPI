using System.Text.RegularExpressions;

namespace ExchangeApi.Optional.Logging.Evidence;

public sealed record EvidenceRunLabel
{
    private static readonly Regex UnsafeChars = new("[^a-zA-Z0-9._-]+", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public EvidenceRunLabel(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = Sanitize(value);
        if (Value is "." or ".." || Value.Length == 0)
        {
            throw new ArgumentException("Evidence run label must contain at least one safe character.", nameof(value));
        }
    }

    public string Value { get; }

    public static string Sanitize(string value)
    {
        var sanitized = UnsafeChars.Replace(value.Trim(), "-").Trim('.', '-', '_');
        while (sanitized.Contains("..", StringComparison.Ordinal))
        {
            sanitized = sanitized.Replace("..", ".", StringComparison.Ordinal);
        }

        return sanitized;
    }

    public override string ToString()
    {
        return Value;
    }
}
