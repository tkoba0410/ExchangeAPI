using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;

public static class GetHealthProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "public", "get-health");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("product_code"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetHealth",
            Summary = "bitFlyer protocol public health",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol public get-health --query-json '{"product_code":"BTC_JPY"}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol public get-health --query-json '{"product_code":"BTC_JPY"}'""",
                "exchangeapi bitflyer protocol public get-health --query-template",
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

        using var bundle = BitflyerClientFactory.CreateProtocolClient(created.Options);
        var call = await bundle.Public.GetHealthCallAsync(
            typed.GetString("product_code"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
