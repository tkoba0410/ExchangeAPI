using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;

public static class GetExecutionsPublicCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "public", "get-executions-public"),
            EndpointId = "GetExecutionsPublic",
            Summary = "bitFlyer native public executions",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.NativeRequest("""{"product_code":null,"count":null,"before":null,"after":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native public get-executions-public --request-json '{"product_code":"BTC_JPY","count":10,"before":null,"after":null}'""",
            CommandOptions =
            [
                CliOptionSpec.Value("product-code"),
                CliOptionSpec.Value("count", "int"),
                CliOptionSpec.Value("before", "long"),
                CliOptionSpec.Value("after", "long"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native public get-executions-public --product-code BTC_JPY --count 10",
                """exchangeapi bitflyer native public get-executions-public --request-json '{"product_code":"BTC_JPY","count":10,"before":null,"after":null}'""",
                "exchangeapi bitflyer native public get-executions-public --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetExecutionsPublicRequest)request;
                return $"product_code={(typed.ProductCode ?? "<omitted>")}, count={(typed.Count?.ToString() ?? "<omitted>")}, before={(typed.Before?.ToString() ?? "<omitted>")}, after={(typed.After?.ToString() ?? "<omitted>")}";
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
            || options.Contains("after");

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
            return JsonInputReader.Deserialize<GetExecutionsPublicRequest>(jsonInput.Content!);
        }

        if (!TryParseIntOption(options, "count", "count", out var count, out var countError))
        {
            return RequestBindingResult.Failure("invalid argument", countError!);
        }

        if (!TryParseLongOption(options, "before", "before", out var before, out var beforeError))
        {
            return RequestBindingResult.Failure("invalid argument", beforeError!);
        }

        if (!TryParseLongOption(options, "after", "after", out var after, out var afterError))
        {
            return RequestBindingResult.Failure("invalid argument", afterError!);
        }

        return RequestBindingResult.Success(new GetExecutionsPublicRequest
        {
            ProductCode = options.GetValue("product-code"),
            Count = count,
            Before = before,
            After = after,
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

        using var bundle = BitflyerClientFactory.CreateNativeClientBundle(created.Options);
        var call = await bundle.Public.GetExecutionsAsync((GetExecutionsPublicRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "public", "get-executions-public"), call);
    }

    private static bool TryParseIntOption(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out int? value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (text is null)
        {
            value = null;
            error = null;
            return true;
        }

        if (int.TryParse(text, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = null;
        error = $"invalid field: {fieldName}";
        return false;
    }

    private static bool TryParseLongOption(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out long? value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (text is null)
        {
            value = null;
            error = null;
            return true;
        }

        if (long.TryParse(text, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = null;
        error = $"invalid field: {fieldName}";
        return false;
    }
}
