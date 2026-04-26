using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class GetBalanceHistoryCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "get-balance-history"),
            EndpointId = "GetBalanceHistory",
            Summary = "bitFlyer native private balance history",
            AuthenticationRequirement = BitflyerCredentialResolver.AuthenticationRequirementText,
            InputContract = CommandInputContract.NativeRequest("""{"currency_code":null,"count":null,"before":null,"after":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private get-balance-history --request-json '{"currency_code":"JPY","count":10}'""",
            CommandOptions =
            [
                CliOptionSpec.Value("currency-code"),
                CliOptionSpec.Value("count", "int"),
                CliOptionSpec.Value("before", "long"),
                CliOptionSpec.Value("after", "long"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private get-balance-history --currency-code JPY --count 10",
                """exchangeapi bitflyer native private get-balance-history --request-json '{"currency_code":"JPY","count":10}'""",
                "exchangeapi bitflyer native private get-balance-history --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetBalanceHistoryRequest)request;
                return $"currency_code={(typed.CurrencyCode ?? "<omitted>")}, count={(typed.Count?.ToString() ?? "<omitted>")}, before={(typed.Before?.ToString() ?? "<omitted>")}, after={(typed.After?.ToString() ?? "<omitted>")}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var hasConvenience = options.Contains("currency-code")
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
            return JsonInputReader.Deserialize<GetBalanceHistoryRequest>(jsonInput.Content!);
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

        return RequestBindingResult.Success(new GetBalanceHistoryRequest
        {
            CurrencyCode = options.GetValue("currency-code"),
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
        var created = BitflyerOptionsFactory.Create(options, environment, requiresCredentials: true);
        if (created.Failure is not null)
        {
            return created.Failure;
        }

        using var bundle = BitflyerClientFactory.CreateNativeClientBundle(created.Options);
        if (bundle.Private is null)
        {
            return ExecutionOutcome.InputError(
                "missing credential",
                BitflyerCredentialResolver.BuildMissingCredentialMessage());
        }

        var call = await bundle.Private.GetBalanceHistoryAsync((GetBalanceHistoryRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "get-balance-history"), call);
    }
}
