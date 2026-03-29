using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class SendParentOrderCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "send-parent-order"),
            EndpointId = "SendParentOrder",
            Summary = "bitFlyer native private send parent order",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{"order_method":"SIMPLE","minute_to_expire":null,"time_in_force":null,"parameters":[{"product_code":"","condition_type":"","side":"","price":null,"size":0,"trigger_price":null,"offset":null}]}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private send-parent-order --request-json '{"order_method":"SIMPLE","time_in_force":"GTC","parameters":[{"product_code":"BTC_JPY","condition_type":"LIMIT","side":"BUY","price":1000000,"size":0.01}]}' --yes""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer native private send-parent-order --request-json '{"order_method":"SIMPLE","time_in_force":"GTC","parameters":[{"product_code":"BTC_JPY","condition_type":"LIMIT","side":"BUY","price":1000000,"size":0.01}]}' --yes""",
                "exchangeapi bitflyer native private send-parent-order --request-file request.json --yes",
                "exchangeapi bitflyer native private send-parent-order --request-template",
            ],
            IsWrite = true,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (SendParentOrderRequest)request;
                var orderMethod = typed.OrderMethod is { } value
                    ? ApiStringEnum<BitflyerOrderMethod>.Format(value)
                    : ApiStringEnum<BitflyerOrderMethod>.Format(ParentOrderMethods.Simple);
                return $"order_method={orderMethod}, parameters={typed.Parameters.Count}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var jsonInput = await JsonInputReader.ReadTextAsync(options, "request-json", "request-file", console, cancellationToken);
        if (jsonInput.Failure is not null)
        {
            return jsonInput.Failure;
        }

        if (!jsonInput.HasValue)
        {
            return RequestBindingResult.Failure(
                "invalid argument",
                "send-parent-order requires --request-json or --request-file in the current phase");
        }

        return JsonInputReader.Deserialize<SendParentOrderRequest>(jsonInput.Content!);
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

        var call = await bundle.Private.SendParentOrderCallAsync((SendParentOrderRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "send-parent-order"), call);
    }
}
