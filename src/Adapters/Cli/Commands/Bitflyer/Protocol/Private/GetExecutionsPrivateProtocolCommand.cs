using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Private;

public static class GetExecutionsPrivateProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "private", "get-executions-private");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("product_code", required: true),
        ProtocolQueryFieldSpec.Int("count"),
        ProtocolQueryFieldSpec.Long("before"),
        ProtocolQueryFieldSpec.Long("after"),
        ProtocolQueryFieldSpec.String("child_order_id"),
        ProtocolQueryFieldSpec.String("child_order_acceptance_id"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetExecutionsPrivate",
            Summary = "bitFlyer protocol private executions",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol private get-executions-private --query-json '{"product_code":"BTC_JPY","count":10}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol private get-executions-private --query-json '{"product_code":"BTC_JPY","count":10}'""",
                "exchangeapi bitflyer protocol private get-executions-private --query-template",
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
                "BITFLYER_API_KEY and BITFLYER_API_SECRET must be set");
        }

        var typed = (ProtocolQueryValues)request;
        var call = await bundle.Private.GetExecutionsCallAsync(
            typed.GetString("product_code")!,
            typed.GetInt("count"),
            typed.GetLong("before"),
            typed.GetLong("after"),
            typed.GetString("child_order_id"),
            typed.GetString("child_order_acceptance_id"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
