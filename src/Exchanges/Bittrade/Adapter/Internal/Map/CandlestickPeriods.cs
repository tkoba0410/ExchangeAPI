using System;
using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Map;

internal static class CandlestickPeriods
{
    private static readonly IReadOnlyDictionary<string, TimeSpan> Table =
        new Dictionary<string, TimeSpan>(StringComparer.OrdinalIgnoreCase)
        {
            ["1min"] = TimeSpan.FromMinutes(1),
            ["5min"] = TimeSpan.FromMinutes(5),
            ["15min"] = TimeSpan.FromMinutes(15),
            ["30min"] = TimeSpan.FromMinutes(30),
            ["60min"] = TimeSpan.FromHours(1),
            ["1day"] = TimeSpan.FromDays(1),
            ["1week"] = TimeSpan.FromDays(7),
        };

    public static bool TryGetTimescale(string code, out TimeSpan timescale)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            timescale = default;
            return false;
        }

        return Table.TryGetValue(code, out timescale);
    }
}
