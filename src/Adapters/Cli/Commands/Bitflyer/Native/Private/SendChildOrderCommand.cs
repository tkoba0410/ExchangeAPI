using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class SendChildOrderCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "send-child-order"),
            EndpointId = "SendChildOrder",
            Summary = "bitFlyer native private send child order",
            AuthenticationRequirement = BitflyerCredentialResolver.AuthenticationRequirementText,
            InputContract = CommandInputContract.NativeRequest("""{"product_code":"","child_order_type":"","side":"","price":null,"size":0,"minute_to_expire":null,"time_in_force":null}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private send-child-order --request-json '{"product_code":"BTC_JPY","child_order_type":"LIMIT","side":"BUY","price":1000000,"size":0.01,"minute_to_expire":43200,"time_in_force":"GTC"}' --yes""",
            CommandOptions =
            [
                CliOptionSpec.Value("product-code"),
                CliOptionSpec.Value("child-order-type"),
                CliOptionSpec.Value("side"),
                CliOptionSpec.Value("price", "decimal"),
                CliOptionSpec.Value("size", "decimal"),
                CliOptionSpec.Value("minute-to-expire", "int"),
                CliOptionSpec.Value("time-in-force"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private send-child-order --product-code BTC_JPY --child-order-type LIMIT --side BUY --price 1000000 --size 0.01 --minute-to-expire 43200 --time-in-force GTC --yes",
                """exchangeapi bitflyer native private send-child-order --request-json '{"product_code":"BTC_JPY","child_order_type":"MARKET","side":"BUY","size":0.01}' --yes""",
                "exchangeapi bitflyer native private send-child-order --request-template",
            ],
            IsWrite = true,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (SendChildOrderRequest)request;
                return $"product_code={typed.ProductCode}, child_order_type={ApiStringEnum<BitflyerChildOrderType>.Format(typed.ChildOrderType)}, side={ApiStringEnum<BitflyerOrderSide>.Format(typed.Side)}, size={typed.Size}";
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
            || options.Contains("child-order-type")
            || options.Contains("side")
            || options.Contains("price")
            || options.Contains("size")
            || options.Contains("minute-to-expire")
            || options.Contains("time-in-force");

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
            return JsonInputReader.Deserialize<SendChildOrderRequest>(jsonInput.Content!);
        }

        if (!OptionValueBinder.TryGetRequiredString(options, "product-code", "product_code", out var productCode, out var productCodeError))
        {
            return RequestBindingResult.Failure("invalid argument", productCodeError);
        }

        if (!OptionValueBinder.TryGetRequiredParsed(
                options,
                "child-order-type",
                "child_order_type",
                ApiStringEnum<BitflyerChildOrderType>.TryParse,
                out BitflyerChildOrderType childOrderType,
                out var childOrderTypeError))
        {
            return RequestBindingResult.Failure("invalid argument", childOrderTypeError);
        }

        if (!OptionValueBinder.TryGetRequiredParsed(
                options,
                "side",
                "side",
                ApiStringEnum<BitflyerOrderSide>.TryParse,
                out BitflyerOrderSide side,
                out var sideError))
        {
            return RequestBindingResult.Failure("invalid argument", sideError);
        }

        if (!OptionValueBinder.TryGetOptionalDecimal(options, "price", "price", out var price, out var priceError))
        {
            return RequestBindingResult.Failure("invalid argument", priceError);
        }

        if (!OptionValueBinder.TryGetRequiredDecimal(options, "size", "size", out var size, out var sizeError))
        {
            return RequestBindingResult.Failure("invalid argument", sizeError);
        }

        if (!OptionValueBinder.TryGetOptionalInt(options, "minute-to-expire", "minute_to_expire", out var minuteToExpire, out var minuteToExpireError))
        {
            return RequestBindingResult.Failure("invalid argument", minuteToExpireError);
        }

        if (!OptionValueBinder.TryGetOptionalParsed(
                options,
                "time-in-force",
                "time_in_force",
                ApiStringEnum<BitflyerTimeInForce>.TryParse,
                out BitflyerTimeInForce? timeInForce,
                out var timeInForceError))
        {
            return RequestBindingResult.Failure("invalid argument", timeInForceError);
        }

        return RequestBindingResult.Success(new SendChildOrderRequest
        {
            ProductCode = productCode,
            ChildOrderType = childOrderType,
            Side = side,
            Price = price,
            Size = size,
            MinuteToExpire = minuteToExpire,
            TimeInForce = timeInForce,
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

        var call = await bundle.Private.SendChildOrderAsync((SendChildOrderRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "send-child-order"), call);
    }
}
