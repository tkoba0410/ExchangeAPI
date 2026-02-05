using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Common.ExchangeInfo.Static;

internal static class StaticJsonLoader
{
    public static T Load<T>(string resourceName)
    {
        if (resourceName is null) throw new ArgumentNullException(nameof(resourceName));

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new InvalidOperationException($"Static exchange info resource not found: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var info = JsonSerializer.Deserialize<T>(json, options);
        if (info is null)
        {
            throw new InvalidOperationException("Failed to deserialize static exchange info.");
        }

        return info;
    }
}
