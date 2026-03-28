using System.Globalization;
using System.Text.Json;
using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Binding;

public static class ProtocolQueryBinder
{
    public static async Task<RequestBindingResult> BindAsync(
        InvocationOptions options,
        IConsole console,
        ProtocolQuerySchema schema,
        CancellationToken cancellationToken)
    {
        var queryInput = await ReadQueryAsync(options, console, cancellationToken);
        if (queryInput.Failure is not null)
        {
            return queryInput.Failure;
        }

        if (!queryInput.HasValue)
        {
            var missingRequiredField = schema.Fields.FirstOrDefault(static field => field.Required);
            if (missingRequiredField is not null)
            {
                return RequestBindingResult.Failure("invalid argument", $"invalid field: {missingRequiredField.Name}");
            }

            return RequestBindingResult.Success(
                new ProtocolQueryValues(
                    schema,
                    new Dictionary<string, JsonElement>(StringComparer.Ordinal),
                    new Dictionary<string, object?>(StringComparer.Ordinal)));
        }

        var failure = ValidateAllowedKeys(queryInput.Query!, schema.Fields.Select(static field => field.Name).ToArray());
        if (failure is not null)
        {
            return failure;
        }

        var typedValues = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var field in schema.Fields)
        {
            failure = field.Kind switch
            {
                ProtocolQueryFieldKind.String when field.Required => TryGetRequiredString(queryInput.Query!, field.Name, out var stringValue)
                    .Also(static (values, name, value) => values[name] = value, typedValues, field.Name, stringValue),
                ProtocolQueryFieldKind.String => TryGetOptionalString(queryInput.Query!, field.Name, out var stringValue)
                    .Also(static (values, name, value) => values[name] = value, typedValues, field.Name, stringValue),
                ProtocolQueryFieldKind.Int when field.Required => TryGetRequiredInt(queryInput.Query!, field.Name, out var intValue)
                    .Also(static (values, name, value) => values[name] = value, typedValues, field.Name, intValue),
                ProtocolQueryFieldKind.Int => TryGetOptionalInt(queryInput.Query!, field.Name, out var intValue)
                    .Also(static (values, name, value) => values[name] = value, typedValues, field.Name, intValue),
                ProtocolQueryFieldKind.Long when field.Required => TryGetRequiredLong(queryInput.Query!, field.Name, out var longValue)
                    .Also(static (values, name, value) => values[name] = value, typedValues, field.Name, longValue),
                ProtocolQueryFieldKind.Long => TryGetOptionalLong(queryInput.Query!, field.Name, out var longValue)
                    .Also(static (values, name, value) => values[name] = value, typedValues, field.Name, longValue),
                _ => RequestBindingResult.Failure("invalid argument", $"invalid field: {field.Name}"),
            };

            if (failure is not null)
            {
                return failure;
            }
        }

        return RequestBindingResult.Success(new ProtocolQueryValues(schema, queryInput.Query!, typedValues));
    }

    public static async Task<(bool HasValue, IReadOnlyDictionary<string, JsonElement>? Query, RequestBindingResult? Failure)> ReadQueryAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var input = await JsonInputReader.ReadTextAsync(options, "query-json", "query-file", console, cancellationToken);
        if (input.Failure is not null)
        {
            return (false, null, input.Failure);
        }

        if (!input.HasValue)
        {
            return (false, null, null);
        }

        try
        {
            using var document = JsonDocument.Parse(input.Content!);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return (false, null, RequestBindingResult.Failure("invalid argument", "query JSON must be an object"));
            }

            var dictionary = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
            foreach (var property in document.RootElement.EnumerateObject())
            {
                dictionary[property.Name] = property.Value.Clone();
            }

            return (true, dictionary, null);
        }
        catch (JsonException ex)
        {
            return (false, null, RequestBindingResult.Failure("invalid argument", ex.Message));
        }
    }

    public static RequestBindingResult? ValidateAllowedKeys(
        IReadOnlyDictionary<string, JsonElement> query,
        params string[] allowedKeys)
    {
        var allowed = new HashSet<string>(allowedKeys, StringComparer.Ordinal);
        foreach (var key in query.Keys)
        {
            if (!allowed.Contains(key))
            {
                return RequestBindingResult.Failure("invalid argument", $"invalid field: {key}");
            }
        }

        return null;
    }

    public static RequestBindingResult? TryGetOptionalString(
        IReadOnlyDictionary<string, JsonElement> query,
        string key,
        out string? value)
    {
        if (!query.TryGetValue(key, out var element))
        {
            value = null;
            return null;
        }

        switch (element.ValueKind)
        {
            case JsonValueKind.Null:
                value = null;
                return null;

            case JsonValueKind.String:
                value = element.GetString();
                return null;

            default:
                value = null;
                return RequestBindingResult.Failure("invalid argument", $"invalid field: {key}");
        }
    }

    public static RequestBindingResult? TryGetRequiredString(
        IReadOnlyDictionary<string, JsonElement> query,
        string key,
        out string? value)
    {
        var failure = TryGetOptionalString(query, key, out value);
        if (failure is not null)
        {
            return failure;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return RequestBindingResult.Failure("invalid argument", $"invalid field: {key}");
        }

        return null;
    }

    public static RequestBindingResult? TryGetOptionalInt(
        IReadOnlyDictionary<string, JsonElement> query,
        string key,
        out int? value)
    {
        if (!query.TryGetValue(key, out var element))
        {
            value = null;
            return null;
        }

        if (element.ValueKind == JsonValueKind.Null)
        {
            value = null;
            return null;
        }

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var number))
        {
            value = number;
            return null;
        }

        if (element.ValueKind == JsonValueKind.String
            && int.TryParse(element.GetString(), CultureInfo.InvariantCulture, out var parsed))
        {
            value = parsed;
            return null;
        }

        value = null;
        return RequestBindingResult.Failure("invalid argument", $"invalid field: {key}");
    }

    public static RequestBindingResult? TryGetRequiredInt(
        IReadOnlyDictionary<string, JsonElement> query,
        string key,
        out int? value)
    {
        var failure = TryGetOptionalInt(query, key, out value);
        if (failure is not null)
        {
            return failure;
        }

        if (value is null)
        {
            return RequestBindingResult.Failure("invalid argument", $"invalid field: {key}");
        }

        return null;
    }

    public static RequestBindingResult? TryGetOptionalLong(
        IReadOnlyDictionary<string, JsonElement> query,
        string key,
        out long? value)
    {
        if (!query.TryGetValue(key, out var element))
        {
            value = null;
            return null;
        }

        if (element.ValueKind == JsonValueKind.Null)
        {
            value = null;
            return null;
        }

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var number))
        {
            value = number;
            return null;
        }

        if (element.ValueKind == JsonValueKind.String
            && long.TryParse(element.GetString(), CultureInfo.InvariantCulture, out var parsed))
        {
            value = parsed;
            return null;
        }

        value = null;
        return RequestBindingResult.Failure("invalid argument", $"invalid field: {key}");
    }

    public static RequestBindingResult? TryGetRequiredLong(
        IReadOnlyDictionary<string, JsonElement> query,
        string key,
        out long? value)
    {
        var failure = TryGetOptionalLong(query, key, out value);
        if (failure is not null)
        {
            return failure;
        }

        if (value is null)
        {
            return RequestBindingResult.Failure("invalid argument", $"invalid field: {key}");
        }

        return null;
    }

    private static RequestBindingResult? Also<T>(
        this RequestBindingResult? failure,
        Action<IDictionary<string, object?>, string, T?> capture,
        IDictionary<string, object?> typedValues,
        string name,
        T? value)
    {
        if (failure is null)
        {
            capture(typedValues, name, value);
        }

        return failure;
    }
}
