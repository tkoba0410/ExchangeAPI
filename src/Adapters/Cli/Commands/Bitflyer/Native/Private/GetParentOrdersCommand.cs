using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class GetParentOrdersCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "get-parent-orders"),
            EndpointId = "GetParentOrders",
            Summary = "bitFlyer native private parent orders",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{"product_code":null,"count":null,"before":null,"after":null,"parent_order_state":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private get-parent-orders --request-json '{"product_code":"BTC_JPY","count":10,"parent_order_state":"ACTIVE"}'""",
            CommandOptions =
            [
                CliOptionSpec.Value("product-code"),
                CliOptionSpec.Value("count", "int"),
                CliOptionSpec.Value("before", "long"),
                CliOptionSpec.Value("after", "long"),
                CliOptionSpec.Value("parent-order-state"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private get-parent-orders --product-code BTC_JPY --parent-order-state ACTIVE --count 10",
                """exchangeapi bitflyer native private get-parent-orders --request-json '{"product_code":"BTC_JPY","count":10,"parent_order_state":"ACTIVE"}'""",
                "exchangeapi bitflyer native private get-parent-orders --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetParentOrdersRequest)request;
                var parentOrderState = typed.ParentOrderState is { } value
                    ? ApiStringEnum<BitflyerOrderState>.Format(value)
                    : "<omitted>";
                return $"product_code={(typed.ProductCode ?? "<omitted>")}, count={(typed.Count?.ToString() ?? "<omitted>")}, parent_order_state={parentOrderState}";
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
            || options.Contains("parent-order-state");

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
            return JsonInputReader.Deserialize<GetParentOrdersRequest>(jsonInput.Content!);
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
                "parent-order-state",
                "parent_order_state",
                ApiStringEnum<BitflyerOrderState>.TryParse,
                out BitflyerOrderState? parentOrderState,
                out var parentOrderStateError))
        {
            return RequestBindingResult.Failure("invalid argument", parentOrderStateError);
        }

        return RequestBindingResult.Success(new GetParentOrdersRequest
        {
            ProductCode = options.GetValue("product-code"),
            Count = count,
            Before = before,
            After = after,
            ParentOrderState = parentOrderState,
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

        var call = await bundle.Private.GetParentOrdersCallAsync((GetParentOrdersRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "get-parent-orders"), call);
    }
}
