using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Binance.Composition.Factory;

namespace ExchangeApi.Adapters.Cli.Commands.Binance.Protocol.Public;

public static class GetKlinesProtocolCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("binance", "protocol", "public", "get-klines"),
            EndpointId = "GetKlines",
            Summary = "Binance protocol public klines",
            AuthenticationRequirement = "none",
            InputMode = CommandInputMode.ProtocolQuery,
            CanonicalJsonExample = """exchangeapi binance protocol public get-klines --query-json '{"symbol":"BTCJPY","interval":"1h","limit":2}'""",
            TemplateJson = """{"symbol":null,"interval":null,"startTime":null,"endTime":null,"timeZone":null,"limit":null}""",
            CommandOptions = [],
            UsageExamples =
            [
                """exchangeapi binance protocol public get-klines --query-json '{"symbol":"BTCJPY","interval":"1h","limit":2}'""",
                "exchangeapi binance protocol public get-klines --query-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetKlinesProtocolCliRequest)request;
                return $"query.symbol={typed.Symbol}, query.interval={typed.Interval}, query.limit={(typed.Limit?.ToString() ?? "<omitted>")}";
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
            return RequestBindingResult.Failure("invalid argument", "invalid field: symbol");
        }

        var failure = ProtocolQueryBinder.ValidateAllowedKeys(queryInput.Query!, "symbol", "interval", "startTime", "endTime", "timeZone", "limit");
        if (failure is not null)
        {
            return failure;
        }

        string? symbol = null;
        string? interval = null;
        long? startTime = null;
        long? endTime = null;
        string? timeZone = null;
        int? limit = null;

        failure = ProtocolQueryBinder.TryGetRequiredString(queryInput.Query!, "symbol", out symbol);
        failure ??= ProtocolQueryBinder.TryGetRequiredString(queryInput.Query!, "interval", out interval);
        failure ??= ProtocolQueryBinder.TryGetOptionalLong(queryInput.Query!, "startTime", out startTime);
        failure ??= ProtocolQueryBinder.TryGetOptionalLong(queryInput.Query!, "endTime", out endTime);
        failure ??= ProtocolQueryBinder.TryGetOptionalString(queryInput.Query!, "timeZone", out timeZone);
        failure ??= ProtocolQueryBinder.TryGetOptionalInt(queryInput.Query!, "limit", out limit);
        if (failure is not null)
        {
            return failure;
        }

        return RequestBindingResult.Success(new GetKlinesProtocolCliRequest
        {
            Symbol = symbol!,
            Interval = interval!,
            StartTime = startTime,
            EndTime = endTime,
            TimeZone = timeZone,
            Limit = limit,
        });
    }

    private static async Task<ExecutionOutcome> ExecuteAsync(
        InvocationOptions options,
        object request,
        IEnvironment environment,
        CancellationToken cancellationToken)
    {
        var created = BinanceOptionsFactory.Create(options);
        if (created.Failure is not null)
        {
            return created.Failure;
        }

        var typed = (GetKlinesProtocolCliRequest)request;

        using var bundle = BinanceClientFactory.CreateProtocolClient(created.Options);
        var call = await bundle.Public.GetKlinesCallAsync(
            typed.Symbol,
            typed.Interval,
            typed.StartTime,
            typed.EndTime,
            typed.TimeZone,
            typed.Limit,
            cancellationToken);
        return ExecutionOutcome.FromProtocolCall(new CommandPath("binance", "protocol", "public", "get-klines"), call);
    }

    private sealed class GetKlinesProtocolCliRequest
    {
        public required string Symbol { get; init; }
        public required string Interval { get; init; }
        public long? StartTime { get; init; }
        public long? EndTime { get; init; }
        public string? TimeZone { get; init; }
        public int? Limit { get; init; }
    }
}
