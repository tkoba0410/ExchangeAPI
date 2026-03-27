using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Binance.Composition.Factory;
using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;

namespace ExchangeApi.Adapters.Cli.Commands.Binance.Native.Public;

public static class GetKlinesCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("binance", "native", "public", "get-klines"),
            EndpointId = "GetKlines",
            Summary = "Binance native public klines",
            AuthenticationRequirement = "none",
            CanonicalJsonExample = """exchangeapi binance native public get-klines --request-json '{"Symbol":"BTCJPY","Interval":"1h","Limit":2}'""",
            TemplateJson = """{"Symbol":"","Interval":"","StartTime":null,"EndTime":null,"TimeZone":null,"Limit":null}""",
            ConvenienceFlags =
            [
                "--symbol <value>",
                "--interval <value>",
                "--limit <int>",
                "--start-time <long>",
                "--end-time <long>",
                "--time-zone <value>",
            ],
            UsageExamples =
            [
                "exchangeapi binance native public get-klines --symbol BTCJPY --interval 1h --limit 2",
                """exchangeapi binance native public get-klines --request-json '{"Symbol":"BTCJPY","Interval":"1h","Limit":2}'""",
                "exchangeapi binance native public get-klines --request-template",
            ],
            IsWrite = false,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (GetKlinesRequest)request;
                return $"Symbol={typed.Symbol}, Interval={typed.Interval}, Limit={(typed.Limit?.ToString() ?? "<omitted>")}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var hasConvenience = options.Contains("symbol")
            || options.Contains("interval")
            || options.Contains("limit")
            || options.Contains("start-time")
            || options.Contains("end-time")
            || options.Contains("time-zone");

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
            return JsonInputReader.Deserialize<GetKlinesRequest>(jsonInput.Content!);
        }

        var symbol = options.GetValue("symbol");
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return RequestBindingResult.Failure("invalid argument", "invalid field: Symbol");
        }

        var interval = options.GetValue("interval");
        if (string.IsNullOrWhiteSpace(interval))
        {
            return RequestBindingResult.Failure("invalid argument", "invalid field: Interval");
        }

        if (!TryParseLongOption(options, "start-time", out var startTime, out var startError))
        {
            return RequestBindingResult.Failure("invalid argument", startError);
        }

        if (!TryParseLongOption(options, "end-time", out var endTime, out var endError))
        {
            return RequestBindingResult.Failure("invalid argument", endError);
        }

        if (!TryParseIntOption(options, "limit", out var limit, out var limitError))
        {
            return RequestBindingResult.Failure("invalid argument", limitError);
        }

        return RequestBindingResult.Success(new GetKlinesRequest
        {
            Symbol = symbol,
            Interval = interval,
            StartTime = startTime,
            EndTime = endTime,
            TimeZone = options.GetValue("time-zone"),
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

        using var bundle = BinanceClientFactory.CreateNativeClient(created.Options);
        var call = await bundle.Public.GetKlinesCallAsync((GetKlinesRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("binance", "native", "public", "get-klines"), call);
    }

    private static bool TryParseIntOption(InvocationOptions options, string optionName, out int? value, out string? error)
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
        error = $"invalid field: {ToPascalCase(optionName)}";
        return false;
    }

    private static bool TryParseLongOption(InvocationOptions options, string optionName, out long? value, out string? error)
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
        error = $"invalid field: {ToPascalCase(optionName)}";
        return false;
    }

    private static string ToPascalCase(string optionName)
    {
        return string.Concat(
            optionName.Split('-', StringSplitOptions.RemoveEmptyEntries)
                .Select(static part => char.ToUpperInvariant(part[0]) + part[1..]));
    }
}
