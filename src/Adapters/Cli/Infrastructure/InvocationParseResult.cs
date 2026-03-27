namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class InvocationParseResult
{
    public required bool IsSuccess { get; init; }
    public required bool ShowHelp { get; init; }
    public required IReadOnlyList<string> PathTokens { get; init; }
    public required InvocationOptions Options { get; init; }
    public string? ErrorSummary { get; init; }
    public string? ErrorDetail { get; init; }
}
