using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;

public static class GetMarketsProtocolCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "protocol", "public", "get-markets"),
            EndpointId = "GetMarkets",
            Summary = "bitFlyer protocol public markets",
            AuthenticationRequirement = "none",
            InputMode = CommandInputMode.ProtocolQuery,
            CanonicalJsonExample = """exchangeapi bitflyer protocol public get-markets --query-json '{}'""",
            TemplateJson = """{}""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol public get-markets --query-json '{}'""",
                "exchangeapi bitflyer protocol public get-markets --query-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static _ => "query=<none>",
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var queryInput = await ProtocolQueryBinder.ReadQueryAsync(options, console, cancellationToken);
        if (queryInput.Failure is not null)
        {
            return queryInput.Failure;
        }

        if (queryInput.HasValue)
        {
            var failure = ProtocolQueryBinder.ValidateAllowedKeys(queryInput.Query!);
            if (failure is not null)
            {
                return failure;
            }
        }

        return RequestBindingResult.Success(new GetMarketsProtocolCliRequest());
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
        return ExecutionOutcome.FromProtocolCall(new CommandPath("bitflyer", "protocol", "public", "get-markets"), call);
    }

    private sealed class GetMarketsProtocolCliRequest
    {
    }
}
