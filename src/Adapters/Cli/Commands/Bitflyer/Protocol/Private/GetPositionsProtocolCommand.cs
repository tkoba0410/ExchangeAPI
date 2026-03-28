using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Private;

public static class GetPositionsProtocolCommand
{
    private static readonly CommandPath Path = new("bitflyer", "protocol", "private", "get-positions");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("product_code", required: true),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetPositions",
            Summary = "bitFlyer protocol private positions",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi bitflyer protocol private get-positions --query-json '{"product_code":"BTC_JPY"}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol private get-positions --query-json '{"product_code":"BTC_JPY"}'""",
                "exchangeapi bitflyer protocol private get-positions --query-template",
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
        var call = await bundle.Private.GetPositionsCallAsync(
            typed.GetString("product_code")!,
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
