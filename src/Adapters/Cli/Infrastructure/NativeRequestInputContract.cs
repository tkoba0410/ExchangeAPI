namespace ExchangeApi.Adapters.Cli.Infrastructure;

internal sealed class NativeRequestInputContract : CommandInputContract
{
    private static readonly CliOptionSpec[] Options =
    [
        CliOptionSpec.Value("request-json", "json"),
        CliOptionSpec.Value("request-file", "path"),
        CliOptionSpec.Flag("request-template"),
    ];

    public NativeRequestInputContract(string templateJson)
    {
        TemplateJson = templateJson;
    }

    public string TemplateJson { get; }

    public override string TemplateOptionName => "request-template";
    public override IReadOnlyList<CliOptionSpec> SupportedOptions => Options;

    public override string BuildTemplateJson()
    {
        return TemplateJson;
    }

    public override bool HasCanonicalInput(InvocationOptions options)
    {
        return options.Contains("request-json") || options.Contains("request-file");
    }
}
