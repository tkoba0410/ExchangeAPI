using ExchangeApi.Adapters.Cli.Infrastructure;
using System.Globalization;

namespace ExchangeApi.Adapters.Cli.Binding;

public static class OptionValueBinder
{
    public delegate bool TryParseValue<T>(string text, out T value);

    public static bool TryGetRequiredString(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out string value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (!string.IsNullOrWhiteSpace(text))
        {
            value = text;
            error = null;
            return true;
        }

        value = string.Empty;
        error = $"invalid field: {fieldName}";
        return false;
    }

    public static bool TryGetOptionalInt(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out int? value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (text is null)
        {
            value = null;
            error = null;
            return true;
        }

        if (int.TryParse(text, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = null;
        error = $"invalid field: {fieldName}";
        return false;
    }

    public static bool TryGetOptionalLong(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out long? value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (text is null)
        {
            value = null;
            error = null;
            return true;
        }

        if (long.TryParse(text, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = null;
        error = $"invalid field: {fieldName}";
        return false;
    }

    public static bool TryGetRequiredParsed<T>(
        InvocationOptions options,
        string optionName,
        string fieldName,
        TryParseValue<T> tryParse,
        out T value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (!string.IsNullOrWhiteSpace(text) && tryParse(text, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = default!;
        error = $"invalid field: {fieldName}";
        return false;
    }

    public static bool TryGetOptionalParsed<T>(
        InvocationOptions options,
        string optionName,
        string fieldName,
        TryParseValue<T> tryParse,
        out T? value,
        out string? error) where T : struct
    {
        var text = options.GetValue(optionName);
        if (text is null)
        {
            value = null;
            error = null;
            return true;
        }

        if (tryParse(text, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = null;
        error = $"invalid field: {fieldName}";
        return false;
    }

    public static bool TryGetRequiredLong(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out long value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (!string.IsNullOrWhiteSpace(text) && long.TryParse(text, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = default;
        error = $"invalid field: {fieldName}";
        return false;
    }

    public static bool TryGetOptionalDecimal(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out decimal? value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (text is null)
        {
            value = null;
            error = null;
            return true;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = null;
        error = $"invalid field: {fieldName}";
        return false;
    }

    public static bool TryGetRequiredDecimal(
        InvocationOptions options,
        string optionName,
        string fieldName,
        out decimal value,
        out string? error)
    {
        var text = options.GetValue(optionName);
        if (!string.IsNullOrWhiteSpace(text) &&
            decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
        {
            value = parsed;
            error = null;
            return true;
        }

        value = default;
        error = $"invalid field: {fieldName}";
        return false;
    }
}
