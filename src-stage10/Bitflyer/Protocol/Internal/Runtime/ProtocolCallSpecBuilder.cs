using System.Text;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;

internal static class ProtocolCallSpecBuilder
{
    public static WireCallSpec Get(string endpointId, string path, string? query) =>
        new(
            Method: HttpMethodNames.Get,
            Path: path,
            EndpointId: endpointId,
            Query: query,
            BodyJson: null,
            Headers: null);

    public static WireCallSpec Post(string endpointId, string path, string bodyJson) =>
        new(
            Method: HttpMethodNames.Post,
            Path: path,
            EndpointId: endpointId,
            Query: null,
            BodyJson: bodyJson,
            Headers: null);

    public static string? BuildQuery(params (string Key, string? Value)[] entries)
    {
        StringBuilder? builder = null;

        foreach (var (key, value) in entries)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            builder ??= new StringBuilder();
            if (builder.Length > 0)
            {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
        }

        return builder?.ToString();
    }
}
