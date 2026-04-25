using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Binance.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Binance.Protocol.Public;

public static class GetKlinesProtocolCommand
{
    private static readonly CommandPath Path = new("binance", "protocol", "public", "get-klines");
    private static readonly ProtocolQuerySchema QuerySchema = new(
    [
        ProtocolQueryFieldSpec.String("symbol", required: true),
        ProtocolQueryFieldSpec.String("interval", required: true),
        ProtocolQueryFieldSpec.Long("startTime"),
        ProtocolQueryFieldSpec.Long("endTime"),
        ProtocolQueryFieldSpec.String("timeZone"),
        ProtocolQueryFieldSpec.Int("limit"),
    ]);

    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = Path,
            EndpointId = "GetKlines",
            Summary = "Binance protocol public klines",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.ProtocolQuery(QuerySchema),
            CanonicalJsonExample = """exchangeapi binance protocol public get-klines --query-json '{"symbol":"BTCJPY","interval":"1h","limit":2}'""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi binance protocol public get-klines --query-json '{"symbol":"BTCJPY","interval":"1h","limit":2}'""",
                "exchangeapi binance protocol public get-klines --query-template",
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
        var created = BinanceOptionsFactory.Create(options);
        if (created.Failure is not null)
        {
            return created.Failure;
        }

        var typed = (ProtocolQueryValues)request;

        using var bundle = BinanceClientFactory.CreateProtocolClientBundle(created.Options);
        var call = await bundle.Public.GetKlinesAsync(
            typed.GetString("symbol")!,
            typed.GetString("interval")!,
            typed.GetLong("startTime"),
            typed.GetLong("endTime"),
            typed.GetString("timeZone"),
            typed.GetInt("limit"),
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(Path, call);
    }
}
