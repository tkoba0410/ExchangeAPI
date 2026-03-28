using System.Globalization;
using System.Text.Json;
using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Binding;

public static class ProtocolQueryBinder
{
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
}
