using System;
using ExchangeApi.Common.Enums;

namespace ExchangeApi.Common.Types;

public static class ExchangeCodeParser
{
    public static ExchangeCode Parse(string value)
    {
        if (TryParse(value, out var exchange))
        {
            return exchange;
        }

        throw new ArgumentException($"Unknown exchange code: '{value}'.", nameof(value));
    }

    public static bool TryParse(string? value, out ExchangeCode exchange)
    {
        exchange = ExchangeCode.Unknown;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = Normalize(value);

        exchange = normalized switch
        {
            "bitflyer" => ExchangeCode.Bitflyer,
            "bittrade" => ExchangeCode.Bittrade,
            "binance" => ExchangeCode.Binance,
            "bitbank" => ExchangeCode.Bitbank,
            "btcbox" => ExchangeCode.Btcbox,
            "coincheck" => ExchangeCode.Coincheck,
            "gmocoin" => ExchangeCode.Gmocoin,
            "okcoin" => ExchangeCode.Okcoin,
            "sandbox" => ExchangeCode.Sandbox,
            _ => ExchangeCode.Unknown
        };

        return exchange != ExchangeCode.Unknown;
    }

    private static string Normalize(string value) =>
        value
            .Trim()
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();
}
