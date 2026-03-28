using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;

public static class GetExecutionsPublicProtocolCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "protocol", "public", "get-executions-public"),
            EndpointId = "GetExecutionsPublic",
            Summary = "bitFlyer protocol public executions",
            AuthenticationRequirement = "none",
            InputMode = CommandInputMode.ProtocolQuery,
            CanonicalJsonExample = """exchangeapi bitflyer protocol public get-executions-public --query-json '{"product_code":"BTC_JPY","count":10}'""",
            TemplateJson = """{"product_code":null,"count":null,"before":null,"after":null}""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi bitflyer protocol public get-executions-public --query-json '{"product_code":"BTC_JPY","count":10}'""",
                "exchangeapi bitflyer protocol public get-executions-public --query-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetExecutionsPublicProtocolCliRequest)request;
                return $"query.product_code={(typed.ProductCode ?? "<omitted>")}, query.count={(typed.Count?.ToString() ?? "<omitted>")}";
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
            return RequestBindingResult.Success(new GetExecutionsPublicProtocolCliRequest());
        }

        var failure = ProtocolQueryBinder.ValidateAllowedKeys(queryInput.Query!, "product_code", "count", "before", "after");
        if (failure is not null)
        {
            return failure;
        }

        string? productCode = null;
        int? count = null;
        long? before = null;
        long? after = null;

        failure = ProtocolQueryBinder.TryGetOptionalString(queryInput.Query!, "product_code", out productCode);
        failure ??= ProtocolQueryBinder.TryGetOptionalInt(queryInput.Query!, "count", out count);
        failure ??= ProtocolQueryBinder.TryGetOptionalLong(queryInput.Query!, "before", out before);
        failure ??= ProtocolQueryBinder.TryGetOptionalLong(queryInput.Query!, "after", out after);
        if (failure is not null)
        {
            return failure;
        }

        return RequestBindingResult.Success(new GetExecutionsPublicProtocolCliRequest
        {
            ProductCode = productCode,
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

        var typed = (GetExecutionsPublicProtocolCliRequest)request;

        using var bundle = BitflyerClientFactory.CreateProtocolClient(created.Options);
        var call = await bundle.Public.GetExecutionsCallAsync(
            typed.ProductCode,
            typed.Count,
            typed.Before,
            typed.After,
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(new CommandPath("bitflyer", "protocol", "public", "get-executions-public"), call);
    }

    private sealed class GetExecutionsPublicProtocolCliRequest
    {
        public string? ProductCode { get; init; }
        public int? Count { get; init; }
        public long? Before { get; init; }
        public long? After { get; init; }
    }
}
