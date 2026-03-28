using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;

public static class GetCorporateLeverageCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "public", "get-corporate-leverage"),
            EndpointId = "GetCorporateLeverage",
            Summary = "bitFlyer native public corporate leverage",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.NativeRequest("""{}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native public get-corporate-leverage --request-json '{}'""",
            CommandOptions = [],
            UsageExamples =
            [
                "exchangeapi bitflyer native public get-corporate-leverage",
                """exchangeapi bitflyer native public get-corporate-leverage --request-json '{}'""",
                "exchangeapi bitflyer native public get-corporate-leverage --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static _ => "empty request",
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var jsonInput = await JsonInputReader.ReadTextAsync(options, "request-json", "request-file", console, cancellationToken);
        if (jsonInput.Failure is not null)
        {
            return jsonInput.Failure;
        }

        if (jsonInput.HasValue)
        {
            return JsonInputReader.Deserialize<GetCorporateLeverageRequest>(jsonInput.Content!);
        }

        return RequestBindingResult.Success(new GetCorporateLeverageRequest());
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

        using var bundle = BitflyerClientFactory.CreateNativeClient(created.Options);
        var call = await bundle.Public.GetCorporateLeverageCallAsync((GetCorporateLeverageRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "public", "get-corporate-leverage"), call);
    }
}
