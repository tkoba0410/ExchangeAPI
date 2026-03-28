using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;

public static class GetFundingRateCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "public", "get-funding-rate"),
            EndpointId = "GetFundingRate",
            Summary = "bitFlyer native public funding rate",
            AuthenticationRequirement = "none",
            InputMode = CommandInputMode.NativeRequest,
            CanonicalJsonExample = """exchangeapi bitflyer native public get-funding-rate --request-json '{"ProductCode":"FX_BTC_JPY"}'""",
            TemplateJson = """{"ProductCode":null}""",
            CommandOptions = [CliOptionSpec.Value("product-code")],
            UsageExamples =
            [
                "exchangeapi bitflyer native public get-funding-rate --product-code FX_BTC_JPY",
                """exchangeapi bitflyer native public get-funding-rate --request-json '{"ProductCode":"FX_BTC_JPY"}'""",
                "exchangeapi bitflyer native public get-funding-rate --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetFundingRateRequest)request;
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
            return JsonInputReader.Deserialize<GetFundingRateRequest>(jsonInput.Content!);
        }

        var productCode = options.GetValue("product-code");
        if (string.IsNullOrWhiteSpace(productCode))
        {
            return RequestBindingResult.Failure("invalid argument", "invalid field: ProductCode");
        }

        return RequestBindingResult.Success(new GetFundingRateRequest
        {
            ProductCode = productCode,
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
        var call = await bundle.Public.GetFundingRateCallAsync((GetFundingRateRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "public", "get-funding-rate"), call);
    }
}
