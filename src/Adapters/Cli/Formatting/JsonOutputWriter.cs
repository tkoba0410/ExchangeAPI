using System.Text.Json;
using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Formatting;

public static class JsonOutputWriter
{
    public static void Write(IConsole console, object? value, bool pretty)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = pretty,
        };

        var json = JsonSerializer.Serialize(value, value?.GetType() ?? typeof(object), options);
        console.WriteOut(json);
    }
}
