namespace ExchangeApi.Adapters.Cli.Infrastructure;

public static class CliOptionCatalog
{
    private static readonly CliOptionSpec[] CommonOptions =
    [
        CliOptionSpec.Value("base-uri", "absolute-uri"),
        CliOptionSpec.Value("timeout-ms", "int"),
        CliOptionSpec.Flag("pretty"),
        CliOptionSpec.Flag("summary"),
        CliOptionSpec.Flag("verbose"),
        CliOptionSpec.Flag("enable-protocol-debug-log"),
        CliOptionSpec.Value("protocol-debug-log-dir", "path"),
    ];

    private static readonly CliOptionSpec[] NativeInputOptions =
    [
        CliOptionSpec.Value("request-json", "json"),
        CliOptionSpec.Value("request-file", "path"),
        CliOptionSpec.Flag("request-template"),
    ];

    private static readonly CliOptionSpec[] ProtocolInputOptions =
    [
        CliOptionSpec.Value("query-json", "json"),
        CliOptionSpec.Value("query-file", "path"),
        CliOptionSpec.Value("body-json", "json"),
        CliOptionSpec.Value("body-file", "path"),
        CliOptionSpec.Flag("query-template"),
        CliOptionSpec.Flag("body-template"),
    ];

    private static readonly CliOptionSpec[] WriteSafetyOptions =
    [
        CliOptionSpec.Flag("yes"),
    ];

    private static readonly CliOptionSpec[] BitflyerSpecificOptions =
    [
        CliOptionSpec.Flag("use-ticker-alias-path"),
    ];

    public static IReadOnlyDictionary<string, CliOptionSpec> BuildAllKnown(IReadOnlyList<CommandDescriptor> commands)
    {
        var dictionary = new Dictionary<string, CliOptionSpec>(StringComparer.Ordinal);
        Add(dictionary, CommonOptions);
        Add(dictionary, NativeInputOptions);
        Add(dictionary, ProtocolInputOptions);
        Add(dictionary, WriteSafetyOptions);
        Add(dictionary, BitflyerSpecificOptions);

        foreach (var command in commands)
        {
            Add(dictionary, command.CommandOptions);
        }

        return dictionary;
    }

    public static ExecutionOutcome? ValidateForCommand(CommandDescriptor descriptor, InvocationOptions options)
    {
        var allowedNames = new HashSet<string>(StringComparer.Ordinal);
        Add(allowedNames, CommonOptions);
        Add(allowedNames, descriptor.CommandOptions);
        Add(allowedNames, GetInputOptions(descriptor.InputMode));

        if (descriptor.IsWrite)
        {
            Add(allowedNames, WriteSafetyOptions);
        }

        if (descriptor.Path.Venue == "bitflyer")
        {
            Add(allowedNames, BitflyerSpecificOptions);
        }

        foreach (var optionName in options.Names)
        {
            if (!allowedNames.Contains(optionName))
            {
                return ExecutionOutcome.InputError(
                    "invalid option",
                    $"unsupported option for {descriptor.Path.Identity}: --{optionName}");
            }
        }

        return null;
    }

    public static IReadOnlyList<CliOptionSpec> GetInputOptions(CommandInputMode inputMode)
    {
        return inputMode switch
        {
            CommandInputMode.NativeRequest => NativeInputOptions,
            CommandInputMode.ProtocolQuery => ProtocolInputOptions
                .Where(static option => option.Name is "query-json" or "query-file" or "query-template")
                .ToArray(),
            CommandInputMode.ProtocolBody => ProtocolInputOptions
                .Where(static option => option.Name is "body-json" or "body-file" or "body-template")
                .ToArray(),
            _ => [],
        };
    }

    private static void Add(IDictionary<string, CliOptionSpec> destination, IEnumerable<CliOptionSpec> source)
    {
        foreach (var option in source)
        {
            destination[option.Name] = option;
        }
    }

    private static void Add(ISet<string> destination, IEnumerable<CliOptionSpec> source)
    {
        foreach (var option in source)
        {
            destination.Add(option.Name);
        }
    }
}
