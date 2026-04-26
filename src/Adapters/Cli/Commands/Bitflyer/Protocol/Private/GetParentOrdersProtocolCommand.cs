using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Private;

public static class GetParentOrdersProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "private", "get-parent-orders");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("product_code"),
        ProtocolQueryFieldSpec.Int("count"),
        ProtocolQueryFieldSpec.Long("before"),
        ProtocolQueryFieldSpec.Long("after"),
        ProtocolQueryFieldSpec.String("parent_order_state"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetParentOrders",
            Summary = "bitFlyer protocol private parent orders",
            AuthenticationRequirement = BitflyerCredentialResolver.AuthenticationRequirementText,
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol private get-parent-orders --query-json '{"product_code":"BTC_JPY","count":10,"parent_order_state":"ACTIVE"}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol private get-parent-orders --query-json '{"product_code":"BTC_JPY","count":10,"parent_order_state":"ACTIVE"}'""",
                "exchangeapi bitflyer protocol private get-parent-orders --query-template",
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

        var typed = (ProtocolQueryValues)request;
        var call = await bundle.Private.GetParentOrdersAsync(
            typed.GetString("product_code"),
            typed.GetInt("count"),
            typed.GetLong("before"),
            typed.GetLong("after"),
            typed.GetString("parent_order_state"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
