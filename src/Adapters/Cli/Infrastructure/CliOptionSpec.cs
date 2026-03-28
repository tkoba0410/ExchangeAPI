namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class CliOptionSpec
{
    private CliOptionSpec(string name, CliOptionKind kind, string? valuePlaceholder)
    {
        Name = name;
        Kind = kind;
        ValuePlaceholder = valuePlaceholder;
    }

    public string Name { get; }
    public CliOptionKind Kind { get; }
    public string? ValuePlaceholder { get; }

    public string DisplayText =>
        Kind == CliOptionKind.Flag
            ? $"--{Name}"
            : $"--{Name} <{ValuePlaceholder ?? "value"}>";

    public static CliOptionSpec Flag(string name)
    {
        return new CliOptionSpec(name, CliOptionKind.Flag, null);
    }

    public static CliOptionSpec Value(string name, string valuePlaceholder = "value")
    {
        return new CliOptionSpec(name, CliOptionKind.Value, valuePlaceholder);
    }
}
