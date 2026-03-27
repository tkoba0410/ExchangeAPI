using ExchangeApi.Adapters.Cli.Wizard;

namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class CommandDescriptor
{
    public required CommandPath Path { get; init; }
    public required string EndpointId { get; init; }
    public required string Summary { get; init; }
    public required string AuthenticationRequirement { get; init; }
    public required string CanonicalJsonExample { get; init; }
    public required string TemplateJson { get; init; }
    public required IReadOnlyList<string> ConvenienceFlags { get; init; }
    public required IReadOnlyList<string> UsageExamples { get; init; }
    public required bool IsWrite { get; init; }
    public WizardDefinition? Wizard { get; init; }
    public required Func<InvocationOptions, IConsole, CancellationToken, Task<RequestBindingResult>> BindRequestAsync { get; init; }
    public required Func<object, string> DescribeRequest { get; init; }
    public required Func<InvocationOptions, object, IEnvironment, CancellationToken, Task<ExecutionOutcome>> ExecuteAsync { get; init; }
}
