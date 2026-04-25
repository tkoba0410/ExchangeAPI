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
        CliOptionSpec.Value("credential-profile", "path"),
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
        Add(dictionary, WriteSafetyOptions);
        Add(dictionary, BitflyerSpecificOptions);

        foreach (var command in commands)
        {
            Add(dictionary, command.InputContract.SupportedOptions);
            Add(dictionary, command.CommandOptions);
        }

        return dictionary;
    }

    public static ExecutionOutcome? ValidateForCommand(CommandDescriptor descriptor, InvocationOptions options)
    {
        var allowedNames = new HashSet<string>(StringComparer.Ordinal);
        Add(allowedNames, CommonOptions);
        Add(allowedNames, descriptor.CommandOptions);
        Add(allowedNames, descriptor.InputContract.SupportedOptions);

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
