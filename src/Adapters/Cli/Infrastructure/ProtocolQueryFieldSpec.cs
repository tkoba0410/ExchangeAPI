namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class ProtocolQueryFieldSpec
{
    private ProtocolQueryFieldSpec(string name, ProtocolQueryFieldKind kind, bool required)
    {
        Name = name;
        Kind = kind;
        Required = required;
    }

    public string Name { get; }
    public ProtocolQueryFieldKind Kind { get; }
    public bool Required { get; }
    public string DisplayKind => Kind switch
    {
        ProtocolQueryFieldKind.String => "string",
        ProtocolQueryFieldKind.Int => "int",
        ProtocolQueryFieldKind.Long => "long",
        _ => "value",
    };

    public static ProtocolQueryFieldSpec String(string name, bool required = false)
    {
        return new ProtocolQueryFieldSpec(name, ProtocolQueryFieldKind.String, required);
    }

    public static ProtocolQueryFieldSpec Int(string name, bool required = false)
    {
        return new ProtocolQueryFieldSpec(name, ProtocolQueryFieldKind.Int, required);
    }

    public static ProtocolQueryFieldSpec Long(string name, bool required = false)
    {
        return new ProtocolQueryFieldSpec(name, ProtocolQueryFieldKind.Long, required);
    }
}
