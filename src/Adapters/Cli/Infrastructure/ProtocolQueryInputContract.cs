namespace ExchangeApi.Adapters.Cli.Infrastructure;

internal sealed class ProtocolQueryInputContract : CommandInputContract
{
    private static readonly CliOptionSpec[] Options =
    [
        CliOptionSpec.Value("query-json", "json"),
        CliOptionSpec.Value("query-file", "path"),
        CliOptionSpec.Flag("query-template"),
    ];

    public ProtocolQueryInputContract(ProtocolQuerySchema schema)
    {
        Schema = schema;
    }

    public ProtocolQuerySchema Schema { get; }

    public override string TemplateOptionName => "query-template";
    public override IReadOnlyList<CliOptionSpec> SupportedOptions => Options;

    public override string BuildTemplateJson()
    {
        return Schema.BuildTemplateJson();
    }

    public override bool HasCanonicalInput(InvocationOptions options)
    {
        return options.Contains("query-json") || options.Contains("query-file");
    }

    public override IReadOnlyList<string> DescribeHelpFields()
    {
        return Schema.DescribeFields();
    }
}
