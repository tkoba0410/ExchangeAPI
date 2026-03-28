namespace ExchangeApi.Adapters.Cli.Infrastructure;

public abstract class CommandInputContract
{
    public static CommandInputContract NativeRequest(string templateJson)
    {
        return new NativeRequestInputContract(templateJson);
    }

    public static CommandInputContract ProtocolQuery(ProtocolQuerySchema schema)
    {
        return new ProtocolQueryInputContract(schema);
    }

    public abstract string TemplateOptionName { get; }
    public abstract IReadOnlyList<CliOptionSpec> SupportedOptions { get; }
    public abstract string BuildTemplateJson();
    public abstract bool HasCanonicalInput(InvocationOptions options);

    public virtual IReadOnlyList<string> DescribeHelpFields()
    {
        return [];
    }
}
