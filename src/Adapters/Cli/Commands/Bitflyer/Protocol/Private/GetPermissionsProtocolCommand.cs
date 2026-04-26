using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Private;

public static class GetPermissionsProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "private", "get-permissions");
    private static readonly ProtocolQuerySchema QuerySchema = new([]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetPermissions",
            Summary = "bitFlyer protocol private permissions",
            AuthenticationRequirement = BitflyerCredentialResolver.AuthenticationRequirementText,
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = "exchangeapi bitflyer protocol private get-permissions --query-json '{}'",
            CommandOptions = [],
            UsageExamples =
            [
                "exchangeapi bitflyer protocol private get-permissions --query-json '{}'",
                "exchangeapi bitflyer protocol private get-permissions --query-template",
            ],
            IsWrite = false,
            BindRequestAsync = static (options, console, cancellationToken) =>
                ProtocolQueryBinder.BindAsync(options, console, QuerySchema, cancellationToken),
            DescribeRequest = static request => ((ProtocolQueryValues)request).Describe(),
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<ExecutionOutcome> ExecuteAsync(
        InvocationOptions options,
        object request,
        IEnvironment environment,
        CancellationToken cancellationToken)
    {
        var created = BitflyerOptionsFactory.Create(options, environment, requiresCredentials: true);
        if (created.Failure is not null)
        {
            return created.Failure;
        }

        using var bundle = BitflyerClientFactory.CreateProtocolClientBundle(created.Options);
        if (bundle.Private is null)
        {
            return ExecutionOutcome.InputError(
                "missing credential",
                BitflyerCredentialResolver.BuildMissingCredentialMessage());
        }

        var call = await bundle.Private.GetPermissionsAsync(cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
