using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Binding;

public static class InvocationParser
{
    public static InvocationParseResult Parse(
        string[] args,
        IReadOnlyDictionary<string, CliOptionSpec> allowedOptions)
    {
        if (args.Length == 0)
        {
            return Success(
                showHelp: true,
                pathTokens: [],
                options: new Dictionary<string, string?>());
        }

        var pathTokens = new List<string>(capacity: 4);
        var options = new Dictionary<string, string?>(StringComparer.Ordinal);
        var helpRequested = false;
        var parsingOptions = false;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg == "help")
            {
                helpRequested = true;
                parsingOptions = true;
                continue;
            }

            if (arg is "--help" or "-h")
            {
                helpRequested = true;
                parsingOptions = true;
                continue;
            }

            if (arg.StartsWith("--", StringComparison.Ordinal))
            {
                parsingOptions = true;
                var optionName = arg[2..];

                if (!allowedOptions.TryGetValue(optionName, out var option))
                {
                    return Failure("invalid option", $"unknown option: --{optionName}. Run: exchangeapi --help");
                }

                if (!options.TryAdd(optionName, null))
                {
                    return Failure("invalid option", $"duplicate option: --{optionName}");
                }

                if (option.Kind == CliOptionKind.Value)
                {
                    if (i + 1 >= args.Length)
                    {
                        return Failure("invalid option", $"missing value for --{optionName}. Example: --{optionName} <value>");
                    }

                    var value = args[++i];
                    if (value.StartsWith("--", StringComparison.Ordinal))
                    {
                        return Failure("invalid option", $"missing value for --{optionName}. Example: --{optionName} <value>");
                    }

                    options[optionName] = value;
                }

                continue;
            }

            if (parsingOptions)
            {
                return Failure("invalid argument", $"unexpected positional argument: {arg}. Put positional tokens before options or run: exchangeapi --help");
            }

            pathTokens.Add(arg);
            if (pathTokens.Count > 4)
            {
                return Failure("invalid argument", "too many command path tokens. Example: exchangeapi bitflyer native public get-ticker --product-code BTC_JPY");
            }
        }

        if (helpRequested || (pathTokens.Count < 4 && options.Count == 0))
        {
            return Success(showHelp: true, pathTokens, options);
        }

        if (pathTokens.Count < 4)
        {
            return Failure("invalid argument", "missing command path. Example: exchangeapi bitflyer native public get-ticker --product-code BTC_JPY");
        }

        return Success(showHelp: false, pathTokens, options);
    }

    private static InvocationParseResult Success(bool showHelp, IReadOnlyList<string> pathTokens, IReadOnlyDictionary<string, string?> options)
    {
        return new InvocationParseResult
        {
            IsSuccess = true,
            ShowHelp = showHelp,
            PathTokens = pathTokens,
            Options = new InvocationOptions(options),
        };
    }

    private static InvocationParseResult Failure(string summary, string detail)
    {
        return new InvocationParseResult
        {
            IsSuccess = false,
            ShowHelp = false,
            PathTokens = [],
            Options = new InvocationOptions(new Dictionary<string, string?>()),
            ErrorSummary = summary,
            ErrorDetail = detail,
        };
    }
}
