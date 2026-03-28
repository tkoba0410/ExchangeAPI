using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;

public static class GetTickerProtocolCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "protocol", "public", "get-ticker"),
            EndpointId = "GetTicker",
            Summary = "bitFlyer protocol public ticker",
            AuthenticationRequirement = "none",
            InputMode = CommandInputMode.ProtocolQuery,
            CanonicalJsonExample = """exchangeapi bitflyer protocol public get-ticker --query-json '{"product_code":"BTC_JPY"}'""",
            TemplateJson = """{"product_code":null}""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol public get-ticker --query-json '{"product_code":"BTC_JPY"}'""",
                "exchangeapi bitflyer protocol public get-ticker --query-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetTickerProtocolCliRequest)request;
                return typed.ProductCode is null ? "query.product_code=<omitted>" : $"query.product_code={typed.ProductCode}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var queryInput = await ProtocolQueryBinder.ReadQueryAsync(options, console, cancellationToken);
        if (queryInput.Failure is not null)
        {
            return queryInput.Failure;
        }

        if (!queryInput.HasValue)
        {
            return RequestBindingResult.Success(new GetTickerProtocolCliRequest());
        }

        var failure = ProtocolQueryBinder.ValidateAllowedKeys(queryInput.Query!, "product_code");
        if (failure is not null)
        {
            return failure;
        }

        failure = ProtocolQueryBinder.TryGetOptionalString(queryInput.Query!, "product_code", out var productCode);
        if (failure is not null)
        {
            return failure;
        }

        return RequestBindingResult.Success(new GetTickerProtocolCliRequest
        {
            ProductCode = productCode,
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

        var typed = (GetTickerProtocolCliRequest)request;

        using var bundle = BitflyerClientFactory.CreateProtocolClient(created.Options);
        var call = await bundle.Public.GetTickerCallAsync(typed.ProductCode, cancellationToken);
        return ExecutionOutcome.FromProtocolCall(new CommandPath("bitflyer", "protocol", "public", "get-ticker"), call);
    }

    private sealed class GetTickerProtocolCliRequest
    {
        public string? ProductCode { get; init; }
    }
}
