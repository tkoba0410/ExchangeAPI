namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;

internal static class ProtocolQueryFactory
{
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
