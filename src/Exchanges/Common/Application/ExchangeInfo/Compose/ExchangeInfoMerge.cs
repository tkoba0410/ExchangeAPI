using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Compose;

internal static class ExchangeInfoMerge
{
    public static IReadOnlyList<TStatic> MergeByKey<TStatic, TDynamic, TKey>(
        IReadOnlyList<TStatic> staticItems,
        IReadOnlyList<TDynamic>? dynamicItems,
        Func<TStatic, TKey> staticKey,
        Func<TDynamic, TKey> dynamicKey,
        Func<TStatic, TDynamic, TStatic> merge,
        Func<TDynamic, TStatic> createFromDynamic,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        if (dynamicItems is null || dynamicItems.Count == 0)
        {
            return staticItems;
        }

        comparer ??= EqualityComparer<TKey>.Default;

        var dynamicByKey = dynamicItems
            .ToDictionary(dynamicKey, item => item, comparer);

        var staticKeys = new HashSet<TKey>(staticItems.Select(staticKey), comparer);

        var merged = staticItems
            .Select(item => dynamicByKey.TryGetValue(staticKey(item), out var dyn) ? merge(item, dyn) : item)
            .ToList();

        foreach (var dynamicItem in dynamicItems)
        {
            var key = dynamicKey(dynamicItem);
            if (staticKeys.Contains(key))
            {
                continue;
            }

            merged.Add(createFromDynamic(dynamicItem));
        }

        return merged;
    }
}
