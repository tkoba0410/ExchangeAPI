using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class CancelChildOrderCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "cancel-child-order"),
            EndpointId = "CancelChildOrder",
            Summary = "bitFlyer native private cancel child order",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{"product_code":"","child_order_id":null,"child_order_acceptance_id":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private cancel-child-order --request-json '{"product_code":"BTC_JPY","child_order_acceptance_id":"JRF20200101-000000-000000"}' --yes""",
            CommandOptions =
            [
                CliOptionSpec.Value("product-code"),
                CliOptionSpec.Value("child-order-id"),
                CliOptionSpec.Value("child-order-acceptance-id"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private cancel-child-order --product-code BTC_JPY --child-order-acceptance-id JRF20200101-000000-000000 --yes",
                """exchangeapi bitflyer native private cancel-child-order --request-json '{"product_code":"BTC_JPY","child_order_acceptance_id":"JRF20200101-000000-000000"}' --yes""",
                "exchangeapi bitflyer native private cancel-child-order --request-template",
            ],
            IsWrite = true,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (CancelChildOrderRequest)request;
                return $"product_code={typed.ProductCode}, child_order_id={(typed.ChildOrderId ?? "<omitted>")}, child_order_acceptance_id={(typed.ChildOrderAcceptanceId ?? "<omitted>")}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var hasConvenience = options.Contains("product-code")
            || options.Contains("child-order-id")
            || options.Contains("child-order-acceptance-id");

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
            return JsonInputReader.Deserialize<CancelChildOrderRequest>(jsonInput.Content!);
        }

        if (!OptionValueBinder.TryGetRequiredString(options, "product-code", "product_code", out var productCode, out var productCodeError))
        {
            return RequestBindingResult.Failure("invalid argument", productCodeError);
        }

        var childOrderId = options.GetValue("child-order-id");
        var childOrderAcceptanceId = options.GetValue("child-order-acceptance-id");
        var hasOrderId = !string.IsNullOrWhiteSpace(childOrderId);
        var hasAcceptanceId = !string.IsNullOrWhiteSpace(childOrderAcceptanceId);
        if (hasOrderId == hasAcceptanceId)
        {
            return RequestBindingResult.Failure(
                "invalid argument",
                "exactly one of child_order_id or child_order_acceptance_id must be specified");
        }

        return RequestBindingResult.Success(new CancelChildOrderRequest
        {
            ProductCode = productCode,
            ChildOrderId = childOrderId,
            ChildOrderAcceptanceId = childOrderAcceptanceId,
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

        var call = await bundle.Private.CancelChildOrderCallAsync((CancelChildOrderRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "cancel-child-order"), call);
    }
}
