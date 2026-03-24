namespace ExchangeApi.Exchanges.Binance.Protocol.Internal.Shared;

internal static class ProtocolQueryFactory
{
    internal static IReadOnlyDictionary<string, string>? Create(params (string Key, string? Value)[] items)
    {
        var query = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var item in items)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                query[item.Key] = item.Value;
            }
        }

        return query.Count == 0 ? null : query;
    }
}
