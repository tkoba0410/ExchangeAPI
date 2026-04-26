namespace ExchangeApi.Optional.Logging.Redaction;

public sealed class RedactionOptions
{
    public string Replacement { get; init; } = "[REDACTED]";

    public IReadOnlySet<string> SensitivePropertyNames { get; init; } = RedactionRules.DefaultSensitivePropertyNames;

    public IReadOnlyCollection<string> SensitiveValues { get; init; } = [];
}
