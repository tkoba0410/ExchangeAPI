namespace ExchangeApi.Adapters.Cli.Wizard;

public enum WizardCanonicalInputKind
{
    RequestJson,
    QueryJson,
}

public sealed class WizardDefinition
{
    public required string Summary { get; init; }
    public required IReadOnlyList<WizardField> Fields { get; init; }
    public WizardCanonicalInputKind CanonicalInputKind { get; init; } = WizardCanonicalInputKind.RequestJson;
    public string? CompletionNote { get; init; }
}
