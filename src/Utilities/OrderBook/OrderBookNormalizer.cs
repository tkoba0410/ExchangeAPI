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
        IEnumerable<GetBoardLevel> bids,
        IEnumerable<GetBoardLevel> asks,
        out GetBoardResponse? normalized,
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

    public static GetBoardResponse Normalize(
        IEnumerable<GetBoardLevel> bids,
        IEnumerable<GetBoardLevel> asks)
    {
        if (bids is null) throw new ArgumentNullException(nameof(bids));
        if (asks is null) throw new ArgumentNullException(nameof(asks));

        return new GetBoardResponse(
            Bids: SortDescending(bids),
            Asks: SortAscending(asks));
    }

    private static IReadOnlyList<GetBoardLevel> SortDescending(IEnumerable<GetBoardLevel> levels)
    {
        var dict = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
        foreach (var level in levels)
        {
            var price = level.Price.Value;
            dict[price] = dict.TryGetValue(price, out var size)
                ? size + level.Size.Value
                : level.Size.Value;
        }
        return dict.Select(kv => new GetBoardLevel(new Price(kv.Key), new Size(kv.Value))).ToList();
    }

    private static IReadOnlyList<GetBoardLevel> SortAscending(IEnumerable<GetBoardLevel> levels)
    {
        var dict = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Default);
        foreach (var level in levels)
        {
            var price = level.Price.Value;
            dict[price] = dict.TryGetValue(price, out var size)
                ? size + level.Size.Value
                : level.Size.Value;
        }
        return dict.Select(kv => new GetBoardLevel(new Price(kv.Key), new Size(kv.Value))).ToList();
    }
}
