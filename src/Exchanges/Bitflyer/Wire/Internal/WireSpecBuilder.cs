using System;
using System.Collections.Generic;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Internal;

internal static class WireSpecBuilder
{
    public static WireCallSpec Get(string endpointId, string path, string? query) =>
        new(Method: HttpMethodNames.Get, Path: path, EndpointId: endpointId, Query: query);

    public static WireCallSpec Post(string endpointId, string path, string? bodyJson) =>
        new(Method: HttpMethodNames.Post, Path: path, EndpointId: endpointId, Query: null, BodyJson: bodyJson);

    public static string? BuildQuery(params (string Key, string? Value)[] entries)
    {
        var parts = new List<string>();
        foreach (var (key, value) in entries)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        return parts.Count == 0 ? null : string.Join("&", parts);
    }
}
