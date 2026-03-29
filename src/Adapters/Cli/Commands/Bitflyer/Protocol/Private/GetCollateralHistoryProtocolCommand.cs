using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Private;

public static class GetCollateralHistoryProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "private", "get-collateral-history");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.Int("count"),
        ProtocolQueryFieldSpec.Long("before"),
        ProtocolQueryFieldSpec.Long("after"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetCollateralHistory",
            Summary = "bitFlyer protocol private collateral history",
            AuthenticationRequirement = BitflyerCredentialResolver.AuthenticationRequirementText,
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol private get-collateral-history --query-json '{"count":10}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol private get-collateral-history --query-json '{"count":10}'""",
                "exchangeapi bitflyer protocol private get-collateral-history --query-template",
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

        using var bundle = BitflyerClientFactory.CreateProtocolClient(created.Options);
        if (bundle.Private is null)
        {
            return ExecutionOutcome.InputError(
                "missing credential",
                BitflyerCredentialResolver.BuildMissingCredentialMessage());
        }

        var typed = (ProtocolQueryValues)request;
        var call = await bundle.Private.GetCollateralHistoryCallAsync(
            typed.GetInt("count"),
            typed.GetLong("before"),
            typed.GetLong("after"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
