using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;

public static class GetBoardProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "public", "get-board");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("product_code"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetBoard",
            Summary = "bitFlyer protocol public board",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol public get-board --query-json '{"product_code":"BTC_JPY"}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol public get-board --query-json '{"product_code":"BTC_JPY"}'""",
                "exchangeapi bitflyer protocol public get-board --query-template",
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
        var call = await bundle.Public.GetBoardAsync(
            typed.GetString("product_code"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
