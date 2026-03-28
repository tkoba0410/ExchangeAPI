using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;

public static class GetMarketsProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "public", "get-markets");
    private static readonly ProtocolQuerySchema QuerySchema = new([]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetMarkets",
            Summary = "bitFlyer protocol public markets",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol public get-markets --query-json '{}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol public get-markets --query-json '{}'""",
                "exchangeapi bitflyer protocol public get-markets --query-template",
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

        using var bundle = BitflyerClientFactory.CreateProtocolClient(created.Options);
        var call = await bundle.Public.GetMarketsCallAsync(cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
