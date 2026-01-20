using System.Linq;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Endpoints.Tests;

internal static class WireCallSpecAssertions
{
    public static void AssertWireCallSpec(
        WireCallSpec request,
        string method,
        string path,
        string endpointId,
        string? query = null,
        string? bodyJson = null)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Path);
        Assert.Equal(endpointId, request.EndpointId);
        Assert.Equal(query, request.Query);
        Assert.Equal(NormalizeJson(bodyJson), NormalizeJson(request.BodyJson));
    }

    private static string? NormalizeJson(string? json) =>
        json is null ? null : string.Concat(json.Where(c => !char.IsWhiteSpace(c)));
}
