using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class GetAddressesCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "get-addresses"),
            EndpointId = "GetAddresses",
            Summary = "bitFlyer native private deposit addresses",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private get-addresses --request-json '{}'""",
            CommandOptions = [],
            UsageExamples =
            [
                "exchangeapi bitflyer native private get-addresses",
                """exchangeapi bitflyer native private get-addresses --request-json '{}'""",
                "exchangeapi bitflyer native private get-addresses --request-template",
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
            return JsonInputReader.Deserialize<GetAddressesRequest>(jsonInput.Content!);
        }

        return RequestBindingResult.Success(new GetAddressesRequest());
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

        using var bundle = BitflyerClientFactory.CreateNativeClient(created.Options);
        if (bundle.Private is null)
        {
            return ExecutionOutcome.InputError(
                "missing credential",
                "BITFLYER_API_KEY and BITFLYER_API_SECRET must be set");
        }

        var call = await bundle.Private.GetAddressesCallAsync((GetAddressesRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "get-addresses"), call);
    }
}
