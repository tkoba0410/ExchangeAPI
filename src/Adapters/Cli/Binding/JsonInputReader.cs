using System.Text.Json;
using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Binding;

public static class JsonInputReader
{
    public static async Task<(bool HasValue, string? Content, RequestBindingResult? Failure)> ReadTextAsync(
        InvocationOptions options,
        string jsonOptionName,
        string fileOptionName,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var inlineJson = options.GetValue(jsonOptionName);
        var filePath = options.GetValue(fileOptionName);

        if (inlineJson is not null && filePath is not null)
        {
            return (false, null, RequestBindingResult.Failure(
                "invalid argument",
                $"--{jsonOptionName} and --{fileOptionName} cannot be used together"));
        }

        if (inlineJson is not null)
        {
            return (true, inlineJson, null);
        }

        if (filePath is null)
        {
            return (false, null, null);
        }

        try
        {
            if (filePath == "-")
            {
                var stdin = await console.ReadStandardInputToEndAsync(cancellationToken);
                return (true, stdin, null);
            }

            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            return (true, content, null);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return (false, null, RequestBindingResult.Failure(
                "invalid argument",
                $"failed to read {filePath}: {ex.Message}"));
        }
    }

    public static RequestBindingResult Deserialize<T>(string json)
    {
        try
        {
            var request = JsonSerializer.Deserialize<T>(json);
            if (request is null)
            {
                return RequestBindingResult.Failure("invalid argument", "request JSON must not be null. Example: --request-json '{\"product_code\":\"BTC_JPY\"}'");
            }

            return RequestBindingResult.Success(request);
        }
        catch (JsonException ex)
        {
            return RequestBindingResult.Failure("invalid argument", $"invalid JSON: {ex.Message}");
        }
    }
}
