using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Static;

internal static class BitflyerStaticExchangeInfoLoader
{
    private const string ResourceName = "ExchangeApi.Exchanges.Bitflyer.Normalized.Static.bitflyer-exchange-info.json";
    private static readonly Lazy<BitflyerStaticExchangeInfo> Cache = new(LoadInternal);

    public static BitflyerStaticExchangeInfo Load() => Cache.Value;

    private static BitflyerStaticExchangeInfo LoadInternal()
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

        var info = JsonSerializer.Deserialize<BitflyerStaticExchangeInfo>(json, options);
        if (info is null)
        {
            throw new InvalidOperationException("Failed to deserialize static exchange info.");
        }

        return info;
    }
}
