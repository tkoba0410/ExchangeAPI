using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;

public static class GetChatsCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "public", "get-chats"),
            EndpointId = "GetChats",
            Summary = "bitFlyer native public chats",
            AuthenticationRequirement = "none",
            InputContract = CommandInputContract.NativeRequest("""{"FromDate":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native public get-chats --request-json '{"FromDate":"2024-01-01T00:00:00Z"}'""",
            CommandOptions = [CliOptionSpec.Value("from-date")],
            UsageExamples =
            [
                "exchangeapi bitflyer native public get-chats",
                "exchangeapi bitflyer native public get-chats --from-date 2024-01-01T00:00:00Z",
                """exchangeapi bitflyer native public get-chats --request-json '{"FromDate":"2024-01-01T00:00:00Z"}'""",
                "exchangeapi bitflyer native public get-chats --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetChatsRequest)request;
                return typed.FromDate is null ? "FromDate=<omitted>" : $"FromDate={typed.FromDate}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var hasConvenience = options.Contains("from-date");
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
            return JsonInputReader.Deserialize<GetChatsRequest>(jsonInput.Content!);
        }

        return RequestBindingResult.Success(new GetChatsRequest
        {
            FromDate = options.GetValue("from-date"),
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
        var call = await bundle.Public.GetChatsCallAsync((GetChatsRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "public", "get-chats"), call);
    }
}
