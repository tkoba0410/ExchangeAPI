using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;

public static class GetExecutionsPublicProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "public", "get-executions-public");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("product_code"),
        ProtocolQueryFieldSpec.Int("count"),
        ProtocolQueryFieldSpec.Long("before"),
        ProtocolQueryFieldSpec.Long("after"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetExecutionsPublic",
            Summary = "bitFlyer protocol public executions",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol public get-executions-public --query-json '{"product_code":"BTC_JPY","count":10}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol public get-executions-public --query-json '{"product_code":"BTC_JPY","count":10}'""",
                "exchangeapi bitflyer protocol public get-executions-public --query-template",
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
        var created = BitflyerOptionsFactory.Create(options, environment, requiresCredentials: false);
        if (created.Failure is not null)
        {
            return created.Failure;
        }

        var typed = (ProtocolQueryValues)request;

        using var bundle = BitflyerClientFactory.CreateProtocolClientBundle(created.Options);
        var call = await bundle.Public.GetExecutionsAsync(
            typed.GetString("product_code"),
            typed.GetInt("count"),
            typed.GetLong("before"),
            typed.GetLong("after"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
