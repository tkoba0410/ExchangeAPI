using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class GetExecutionsPrivateCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "get-executions-private"),
            EndpointId = "GetExecutionsPrivate",
            Summary = "bitFlyer native private executions",
            AuthenticationRequirement = BitflyerCredentialResolver.AuthenticationRequirementText,
            InputContract = CommandInputContract.NativeRequest("""{"product_code":"","count":null,"before":null,"after":null,"child_order_id":null,"child_order_acceptance_id":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private get-executions-private --request-json '{"product_code":"BTC_JPY","count":10}'""",
            CommandOptions =
            [
                CliOptionSpec.Value("product-code"),
                CliOptionSpec.Value("count", "int"),
                CliOptionSpec.Value("before", "long"),
                CliOptionSpec.Value("after", "long"),
                CliOptionSpec.Value("child-order-id"),
                CliOptionSpec.Value("child-order-acceptance-id"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private get-executions-private --product-code BTC_JPY --count 10",
                """exchangeapi bitflyer native private get-executions-private --request-json '{"product_code":"BTC_JPY","count":10}'""",
                "exchangeapi bitflyer native private get-executions-private --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetExecutionsRequest)request;
                return $"product_code={typed.ProductCode}, count={(typed.Count?.ToString() ?? "<omitted>")}, before={(typed.Before?.ToString() ?? "<omitted>")}, after={(typed.After?.ToString() ?? "<omitted>")}";
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
            return JsonInputReader.Deserialize<GetExecutionsRequest>(jsonInput.Content!);
        }

        if (!OptionValueBinder.TryGetRequiredString(options, "product-code", "product_code", out var productCode, out var productCodeError))
        {
            return RequestBindingResult.Failure("invalid argument", productCodeError);
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

        return RequestBindingResult.Success(new GetExecutionsRequest
        {
            ProductCode = productCode,
            Count = count,
            Before = before,
            After = after,
            ChildOrderId = options.GetValue("child-order-id"),
            ChildOrderAcceptanceId = options.GetValue("child-order-acceptance-id"),
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
                BitflyerCredentialResolver.BuildMissingCredentialMessage());
        }

        var call = await bundle.Private.GetExecutionsCallAsync((GetExecutionsRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "get-executions-private"), call);
    }
}
