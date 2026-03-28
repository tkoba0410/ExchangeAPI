using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class GetDepositsCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "get-deposits"),
            EndpointId = "GetDeposits",
            Summary = "bitFlyer native private deposits",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{"count":null,"before":null,"after":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private get-deposits --request-json '{"count":10}'""",
            CommandOptions =
            [
                CliOptionSpec.Value("count", "int"),
                CliOptionSpec.Value("before", "long"),
                CliOptionSpec.Value("after", "long"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private get-deposits --count 10",
                """exchangeapi bitflyer native private get-deposits --request-json '{"count":10}'""",
                "exchangeapi bitflyer native private get-deposits --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetDepositsRequest)request;
                return $"count={(typed.Count?.ToString() ?? "<omitted>")}, before={(typed.Before?.ToString() ?? "<omitted>")}, after={(typed.After?.ToString() ?? "<omitted>")}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var hasConvenience = options.Contains("count")
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
            return JsonInputReader.Deserialize<GetDepositsRequest>(jsonInput.Content!);
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

        return RequestBindingResult.Success(new GetDepositsRequest
        {
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

        using var bundle = BitflyerClientFactory.CreateNativeClient(created.Options);
        if (bundle.Private is null)
        {
            return ExecutionOutcome.InputError(
                "missing credential",
                "BITFLYER_API_KEY and BITFLYER_API_SECRET must be set");
        }

        var call = await bundle.Private.GetDepositsCallAsync((GetDepositsRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "get-deposits"), call);
    }
}
