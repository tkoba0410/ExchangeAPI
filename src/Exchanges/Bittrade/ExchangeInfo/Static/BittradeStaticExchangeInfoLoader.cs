using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Static;

internal static class BittradeStaticExchangeInfoLoader
{
    private const string ResourceName =
        "ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Static.bittrade-exchange-info.json";
    private static readonly Lazy<BittradeStaticExchangeInfo> Cache = new(LoadInternal);

    public static BittradeStaticExchangeInfo Load() => Cache.Value;

    private static BittradeStaticExchangeInfo LoadInternal()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(ResourceName);
        if (stream is null)
        {
            throw new InvalidOperationException($"Static exchange info resource not found: {ResourceName}");
        }

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var info = JsonSerializer.Deserialize<BittradeStaticExchangeInfo>(json, options);
        if (info is null)
        {
            throw new InvalidOperationException("Failed to deserialize static exchange info.");
        }

        return info;
    }
}
