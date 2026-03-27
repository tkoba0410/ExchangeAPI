using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Adapters.Cli.Wizard;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class CancelAllChildOrdersCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "cancel-all-child-orders"),
            EndpointId = "CancelAllChildOrders",
            Summary = "bitFlyer native private cancel all child orders",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            CanonicalJsonExample = """exchangeapi bitflyer native private cancel-all-child-orders --request-json '{"product_code":"BTC_JPY"}' --yes""",
            TemplateJson = """{"product_code":""}""",
            ConvenienceFlags = ["--product-code <value>"],
            UsageExamples =
            [
                "exchangeapi bitflyer native private cancel-all-child-orders --product-code BTC_JPY --yes",
                """exchangeapi bitflyer native private cancel-all-child-orders --request-json '{"product_code":"BTC_JPY"}' --yes""",
                "exchangeapi bitflyer native private cancel-all-child-orders --request-template",
            ],
            IsWrite = true,
            Wizard = new WizardDefinition
            {
                Summary = "Collects product_code and prints an equivalent canonical request-json command.",
                Fields =
                [
                    new WizardField
                    {
                        OptionName = "product-code",
                        Prompt = "product_code",
                        Required = true,
                    },
                ],
                CompletionNote = "write command note: copied command will still require interactive confirmation unless you append --yes",
            },
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (CancelAllChildOrdersRequest)request;
                return $"product_code={typed.ProductCode}";
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
            return JsonInputReader.Deserialize<CancelAllChildOrdersRequest>(jsonInput.Content!);
        }

        var productCode = options.GetValue("product-code");
        if (string.IsNullOrWhiteSpace(productCode))
        {
            return RequestBindingResult.Failure("invalid argument", "invalid field: product_code");
        }

        return RequestBindingResult.Success(new CancelAllChildOrdersRequest
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

        var call = await bundle.Private.CancelAllChildOrdersCallAsync((CancelAllChildOrdersRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "cancel-all-child-orders"), call);
    }
}
