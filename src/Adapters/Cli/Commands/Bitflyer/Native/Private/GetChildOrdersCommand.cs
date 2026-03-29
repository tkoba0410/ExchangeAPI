using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class GetChildOrdersCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "get-child-orders"),
            EndpointId = "GetChildOrders",
            Summary = "bitFlyer native private child orders",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{"product_code":null,"count":null,"before":null,"after":null,"child_order_state":null,"child_order_id":null,"child_order_acceptance_id":null,"parent_order_id":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private get-child-orders --request-json '{"product_code":"BTC_JPY","count":10,"child_order_state":"ACTIVE"}'""",
            CommandOptions =
            [
                CliOptionSpec.Value("product-code"),
                CliOptionSpec.Value("count", "int"),
                CliOptionSpec.Value("before", "long"),
                CliOptionSpec.Value("after", "long"),
                CliOptionSpec.Value("child-order-state"),
                CliOptionSpec.Value("child-order-id"),
                CliOptionSpec.Value("child-order-acceptance-id"),
                CliOptionSpec.Value("parent-order-id"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private get-child-orders --product-code BTC_JPY --child-order-state ACTIVE --count 10",
                """exchangeapi bitflyer native private get-child-orders --request-json '{"product_code":"BTC_JPY","count":10,"child_order_state":"ACTIVE"}'""",
                "exchangeapi bitflyer native private get-child-orders --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetChildOrdersRequest)request;
                var childOrderState = typed.ChildOrderState is { } value
                    ? ApiStringEnum<BitflyerOrderState>.Format(value)
                    : "<omitted>";
                return $"product_code={(typed.ProductCode ?? "<omitted>")}, count={(typed.Count?.ToString() ?? "<omitted>")}, child_order_state={childOrderState}";
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
            || options.Contains("count")
            || options.Contains("before")
            || options.Contains("after")
            || options.Contains("child-order-state")
            || options.Contains("child-order-id")
            || options.Contains("child-order-acceptance-id")
            || options.Contains("parent-order-id");

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
            return JsonInputReader.Deserialize<GetChildOrdersRequest>(jsonInput.Content!);
        }

        if (!OptionValueBinder.TryGetOptionalInt(options, "count", "count", out var count, out var countError))
        {
            return RequestBindingResult.Failure("invalid argument", countError);
        }

        if (!OptionValueBinder.TryGetOptionalLong(options, "before", "before", out var before, out var beforeError))
        {
            return RequestBindingResult.Failure("invalid argument", beforeError);
        }

        if (!OptionValueBinder.TryGetOptionalLong(options, "after", "after", out var after, out var afterError))
        {
            return RequestBindingResult.Failure("invalid argument", afterError);
        }

        if (!OptionValueBinder.TryGetOptionalParsed(
                options,
                "child-order-state",
                "child_order_state",
                ApiStringEnum<BitflyerOrderState>.TryParse,
                out BitflyerOrderState? childOrderState,
                out var childOrderStateError))
        {
            return RequestBindingResult.Failure("invalid argument", childOrderStateError);
        }

        return RequestBindingResult.Success(new GetChildOrdersRequest
        {
            ProductCode = options.GetValue("product-code"),
            Count = count,
            Before = before,
            After = after,
            ChildOrderState = childOrderState,
            ChildOrderId = options.GetValue("child-order-id"),
            ChildOrderAcceptanceId = options.GetValue("child-order-acceptance-id"),
            ParentOrderId = options.GetValue("parent-order-id"),
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

        var call = await bundle.Private.GetChildOrdersCallAsync((GetChildOrdersRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "get-child-orders"), call);
    }
}
