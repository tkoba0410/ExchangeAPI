using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Private;

public static class GetParentOrderProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "private", "get-parent-order");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("parent_order_id"),
        ProtocolQueryFieldSpec.String("parent_order_acceptance_id"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetParentOrder",
            Summary = "bitFlyer protocol private parent order",
            AuthenticationRequirement = BitflyerCredentialResolver.AuthenticationRequirementText,
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol private get-parent-order --query-json '{"parent_order_acceptance_id":"JRF20200101-000000-000000"}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol private get-parent-order --query-json '{"parent_order_acceptance_id":"JRF20200101-000000-000000"}'""",
                "exchangeapi bitflyer protocol private get-parent-order --query-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request => ((ProtocolQueryValues)request).Describe(),
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var bound = await ProtocolQueryBinder.BindAsync(options, console, QuerySchema, cancellationToken);
        if (!bound.IsSuccess)
        {
            return bound;
        }

        var typed = (ProtocolQueryValues)bound.Request!;
        var hasParentOrderId = !string.IsNullOrWhiteSpace(typed.GetString("parent_order_id"));
        var hasAcceptanceId = !string.IsNullOrWhiteSpace(typed.GetString("parent_order_acceptance_id"));
        if (hasParentOrderId == hasAcceptanceId)
        {
            return RequestBindingResult.Failure(
                "invalid argument",
                "exactly one of parent_order_id or parent_order_acceptance_id must be specified");
        }

        return bound;
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
        var call = await bundle.Private.GetParentOrderCallAsync(
            typed.GetString("parent_order_id"),
            typed.GetString("parent_order_acceptance_id"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
