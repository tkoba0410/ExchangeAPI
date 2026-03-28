using System.Text.Json;

namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class ProtocolQuerySchema
{
    public ProtocolQuerySchema(IReadOnlyList<ProtocolQueryFieldSpec> fields)
    {
        Fields = fields;
    }

    public IReadOnlyList<ProtocolQueryFieldSpec> Fields { get; }

    public string BuildTemplateJson()
    {
        var dictionary = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var field in Fields)
        {
            dictionary[field.Name] = null;
        }

        return JsonSerializer.Serialize(dictionary);
    }

    public string Describe(ProtocolQueryValues values)
    {
        if (Fields.Count == 0)
        {
            return "query=<none>";
        }

        return string.Join(
            ", ",
            Fields.Select(field =>
            {
                var rendered = field.Kind switch
                {
                    ProtocolQueryFieldKind.String => values.GetString(field.Name),
                    ProtocolQueryFieldKind.Int => values.GetInt(field.Name)?.ToString(),
                    ProtocolQueryFieldKind.Long => values.GetLong(field.Name)?.ToString(),
                    _ => null,
                };

                return $"query.{field.Name}={(rendered ?? "<omitted>")}";
            }));
    }

    public IReadOnlyList<string> DescribeFields()
    {
        if (Fields.Count == 0)
        {
            return ["<none>"];
        }

        return Fields
            .Select(static field => $"{field.Name} <{field.DisplayKind}> {(field.Required ? "required" : "optional")}")
            .ToArray();
    }
}
