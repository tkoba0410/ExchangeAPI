using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Utilities.OrderBook;

public static class OrderBookNormalizer
{
    public static bool TryNormalize(
        IEnumerable<BoardLevel> bids,
        IEnumerable<BoardLevel> asks,
        out BoardResponse? normalized,
        out CallError? error)
    {
        if (bids is null || asks is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "OrderBook normalization input is null.");
            return false;
        }

        try
        {
            normalized = Normalize(bids, asks);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "OrderBook normalization failed.", ex);
            return false;
        }
    }

    public static BoardResponse Normalize(
        IEnumerable<BoardLevel> bids,
        IEnumerable<BoardLevel> asks)
    {
        if (bids is null) throw new ArgumentNullException(nameof(bids));
        if (asks is null) throw new ArgumentNullException(nameof(asks));

        return new BoardResponse(
            Bids: SortDescending(bids),
            Asks: SortAscending(asks));
    }

    private static IReadOnlyList<BoardLevel> SortDescending(IEnumerable<BoardLevel> levels)
    {
        var dict = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
        foreach (var level in levels)
        {
            var price = level.Price.Value;
            dict[price] = dict.TryGetValue(price, out var size)
                ? size + level.Size.Value
                : level.Size.Value;
        }
        return dict.Select(kv => new BoardLevel(new Price(kv.Key), new Size(kv.Value))).ToList();
    }

    private static IReadOnlyList<BoardLevel> SortAscending(IEnumerable<BoardLevel> levels)
    {
        var dict = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Default);
        foreach (var level in levels)
        {
            var price = level.Price.Value;
            dict[price] = dict.TryGetValue(price, out var size)
                ? size + level.Size.Value
                : level.Size.Value;
        }
        return dict.Select(kv => new BoardLevel(new Price(kv.Key), new Size(kv.Value))).ToList();
    }
}
