namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;

internal static class ProtocolHeaderReader
{
    internal static IReadOnlyDictionary<string, string[]> ReadHeaders(HttpResponseMessage response)
    {
        var headers = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var header in response.Headers)
        {
            headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in response.Content.Headers)
        {
            headers[header.Key] = header.Value.ToArray();
        }

        return headers;
    }
}
