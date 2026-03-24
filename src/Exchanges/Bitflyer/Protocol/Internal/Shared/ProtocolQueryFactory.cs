namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;

internal static class ProtocolQueryFactory
{
    internal static IReadOnlyDictionary<string, string>? Create(params (string Key, string? Value)[] values)
    {
        var query = new Dictionary<string, string>();

        foreach (var (key, value) in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                query[key] = value;
            }
        }

        return query.Count == 0 ? null : query;
    }

    internal static IReadOnlyDictionary<string, string>? CreateSingle(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return new Dictionary<string, string>
        {
            [key] = value,
        };
    }
}
