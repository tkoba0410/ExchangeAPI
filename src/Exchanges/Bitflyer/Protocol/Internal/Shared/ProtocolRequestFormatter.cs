using System.Net;
using System.Text;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;

internal static class ProtocolRequestFormatter
{
    internal static Uri ToRequestUri(Uri baseUri, ProtocolRequest request)
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        ArgumentNullException.ThrowIfNull(request);

        return new Uri(baseUri, ToPathAndQuery(request));
    }

    internal static string ToPathAndQuery(ProtocolRequest request)
    {
        if (request.Query is null || request.Query.Count == 0)
        {
            return request.Path;
        }

        var builder = new StringBuilder(request.Path);
        builder.Append('?');

        var first = true;
        foreach (var pair in request.Query.OrderBy(static x => x.Key, StringComparer.Ordinal))
        {
            if (!first)
            {
                builder.Append('&');
            }

            builder.Append(WebUtility.UrlEncode(pair.Key));
            builder.Append('=');
            builder.Append(WebUtility.UrlEncode(pair.Value));
            first = false;
        }

        return builder.ToString();
    }
}
