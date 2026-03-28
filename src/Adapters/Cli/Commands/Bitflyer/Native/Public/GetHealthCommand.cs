using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;

public static class GetHealthCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "public", "get-health"),
            EndpointId = "GetHealth",
            Summary = "bitFlyer native public health",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.NativeRequest("""{"ProductCode":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native public get-health --request-json '{"ProductCode":"BTC_JPY"}'""",
            CommandOptions = [CliOptionSpec.Value("product-code")],
            UsageExamples =
            [
                "exchangeapi bitflyer native public get-health --product-code BTC_JPY",
                """exchangeapi bitflyer native public get-health --request-json '{"ProductCode":"BTC_JPY"}'""",
                "exchangeapi bitflyer native public get-health --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetHealthRequest)request;
                return typed.ProductCode is null ? "ProductCode=<omitted>" : $"ProductCode={typed.ProductCode}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var hasConvenience = options.Contains("product-code");
        var jsonInput = await JsonInputReader.ReadTextAsync(options, "request-json", "request-file", console, cancellationToken);
        if (jsonInput.Failure is not null)
        {
            return jsonInput.Failure;
        }

        if (jsonInput.HasValue && hasConvenience)
        {
            return RequestBindingResult.Failure(
                "invalid argument",
                "--request-json/--request-file and convenience flags cannot be used together");
        }

        if (jsonInput.HasValue)
        {
            return JsonInputReader.Deserialize<GetHealthRequest>(jsonInput.Content!);
        }

        return RequestBindingResult.Success(new GetHealthRequest
        {
            ProductCode = options.GetValue("product-code"),
        });
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
        var call = await bundle.Public.GetHealthCallAsync((GetHealthRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "public", "get-health"), call);
    }
}
