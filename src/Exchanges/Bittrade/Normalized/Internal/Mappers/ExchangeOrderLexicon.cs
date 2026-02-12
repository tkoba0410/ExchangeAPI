using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;

internal static class ExchangeOrderLexicon
{
    private static readonly IReadOnlyDictionary<ExchangeOrderType, string> OrderTypeToRaw =
        new Dictionary<ExchangeOrderType, string>
        {
            [ExchangeOrderType.BuyLimit] = "buy-limit",
            [ExchangeOrderType.SellLimit] = "sell-limit",
            [ExchangeOrderType.BuyMarket] = "buy-market",
            [ExchangeOrderType.SellMarket] = "sell-market",
            [ExchangeOrderType.BuyLimitMaker] = "buy-limit-maker",
            [ExchangeOrderType.SellLimitMaker] = "sell-limit-maker",
            [ExchangeOrderType.BuyIoc] = "buy-ioc",
            [ExchangeOrderType.SellIoc] = "sell-ioc",
        };

    private static readonly IReadOnlyDictionary<string, ExchangeOrderType> RawToOrderType =
        BuildRawToOrderType();

    private static readonly IReadOnlyDictionary<string, ExchangeOrderState> RawToOrderState =
        new Dictionary<string, ExchangeOrderState>
        {
            ["submitted"] = ExchangeOrderState.Submitted,
            ["partial-filled"] = ExchangeOrderState.PartialFilled,
            ["filled"] = ExchangeOrderState.Filled,
            ["partial-canceled"] = ExchangeOrderState.PartialCanceled,
            ["canceled"] = ExchangeOrderState.Canceled,
        };

    public static bool TryToRawOrderType(ExchangeOrderType type, out string raw) =>
        OrderTypeToRaw.TryGetValue(type, out raw!);

    public static bool TryParseOrderType(string raw, out ExchangeOrderType parsed) =>
        RawToOrderType.TryGetValue(raw, out parsed);

    public static Closed<ExchangeOrderType> ParseOrderTypeClosed(string? raw)
    {
        if (raw is not null && RawToOrderType.TryGetValue(raw, out var parsed))
        {
            return Closed<ExchangeOrderType>.KnownValue(parsed);
        }

        return Closed<ExchangeOrderType>.UnknownValue(raw ?? string.Empty);
    }

    public static bool TryParseOrderState(string raw, out ExchangeOrderState parsed) =>
        RawToOrderState.TryGetValue(raw, out parsed);

    public static Closed<ExchangeOrderState> ParseOrderStateClosed(string? raw)
    {
        if (raw is not null && RawToOrderState.TryGetValue(raw, out var parsed))
        {
            return Closed<ExchangeOrderState>.KnownValue(parsed);
        }

        return Closed<ExchangeOrderState>.UnknownValue(raw ?? string.Empty);
    }

    private static IReadOnlyDictionary<string, ExchangeOrderType> BuildRawToOrderType()
    {
        var map = new Dictionary<string, ExchangeOrderType>();
        foreach (var pair in OrderTypeToRaw)
        {
            map[pair.Value] = pair.Key;
        }

        return map;
    }
}
