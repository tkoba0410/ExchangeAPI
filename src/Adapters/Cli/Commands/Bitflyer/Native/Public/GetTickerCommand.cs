using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Adapters.Cli.Wizard;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;

public static class GetTickerCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "public", "get-ticker"),
            EndpointId = "GetTicker",
            Summary = "bitFlyer native public ticker",
            AuthenticationRequirement = "none",
            InputMode = CommandInputMode.NativeRequest,
            CanonicalJsonExample = """exchangeapi bitflyer native public get-ticker --request-json '{"product_code":"BTC_JPY"}'""",
            TemplateJson = """{"product_code":null}""",
            CommandOptions = [CliOptionSpec.Value("product-code")],
            UsageExamples =
            [
                "exchangeapi bitflyer native public get-ticker --product-code BTC_JPY",
                """exchangeapi bitflyer native public get-ticker --request-json '{"product_code":"BTC_JPY"}'""",
                "exchangeapi bitflyer native public get-ticker --request-template",
            ],
            IsWrite = false,
            Wizard = new WizardDefinition
            {
                Summary = "Collects a product code and prints an equivalent canonical request-json command.",
                Fields =
                [
                    new WizardField
                    {
                        OptionName = "product-code",
                        Prompt = "product_code",
                        Required = false,
                        Hint = "leave blank to omit",
                    },
                ],
            },
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetTickerRequest)request;
                return typed.ProductCode is null ? "product_code=<omitted>" : $"product_code={typed.ProductCode}";
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
            return JsonInputReader.Deserialize<GetTickerRequest>(jsonInput.Content!);
        }

        return RequestBindingResult.Success(new GetTickerRequest
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
        var call = await bundle.Public.GetTickerCallAsync((GetTickerRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "public", "get-ticker"), call);
    }
}
