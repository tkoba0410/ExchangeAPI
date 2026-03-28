using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class CancelParentOrderCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "cancel-parent-order"),
            EndpointId = "CancelParentOrder",
            Summary = "bitFlyer native private cancel parent order",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{"product_code":"","parent_order_id":null,"parent_order_acceptance_id":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private cancel-parent-order --request-json '{"product_code":"BTC_JPY","parent_order_acceptance_id":"JRF20200101-000000-000000"}' --yes""",
            CommandOptions =
            [
                CliOptionSpec.Value("product-code"),
                CliOptionSpec.Value("parent-order-id"),
                CliOptionSpec.Value("parent-order-acceptance-id"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private cancel-parent-order --product-code BTC_JPY --parent-order-acceptance-id JRF20200101-000000-000000 --yes",
                """exchangeapi bitflyer native private cancel-parent-order --request-json '{"product_code":"BTC_JPY","parent_order_acceptance_id":"JRF20200101-000000-000000"}' --yes""",
                "exchangeapi bitflyer native private cancel-parent-order --request-template",
            ],
            IsWrite = true,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (CancelParentOrderRequest)request;
                return $"product_code={typed.ProductCode}, parent_order_id={(typed.ParentOrderId ?? "<omitted>")}, parent_order_acceptance_id={(typed.ParentOrderAcceptanceId ?? "<omitted>")}";
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
            || options.Contains("parent-order-id")
            || options.Contains("parent-order-acceptance-id");

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
            return JsonInputReader.Deserialize<CancelParentOrderRequest>(jsonInput.Content!);
        }

        if (!OptionValueBinder.TryGetRequiredString(options, "product-code", "product_code", out var productCode, out var productCodeError))
        {
            return RequestBindingResult.Failure("invalid argument", productCodeError);
        }

        var parentOrderId = options.GetValue("parent-order-id");
        var parentOrderAcceptanceId = options.GetValue("parent-order-acceptance-id");
        var hasParentOrderId = !string.IsNullOrWhiteSpace(parentOrderId);
        var hasAcceptanceId = !string.IsNullOrWhiteSpace(parentOrderAcceptanceId);
        if (hasParentOrderId == hasAcceptanceId)
        {
            return RequestBindingResult.Failure(
                "invalid argument",
                "exactly one of parent_order_id or parent_order_acceptance_id must be specified");
        }

        return RequestBindingResult.Success(new CancelParentOrderRequest
        {
            ProductCode = productCode,
            ParentOrderId = parentOrderId,
            ParentOrderAcceptanceId = parentOrderAcceptanceId,
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

        var call = await bundle.Private.CancelParentOrderCallAsync((CancelParentOrderRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "cancel-parent-order"), call);
    }
}
