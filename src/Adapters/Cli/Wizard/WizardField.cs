namespace ExchangeApi.Adapters.Cli.Wizard;

public sealed class WizardField
{
    public required string OptionName { get; init; }
    public required string Prompt { get; init; }
    public bool Required { get; init; }
    public string? Hint { get; init; }
}
