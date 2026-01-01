using System.Linq;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Endpoints.Tests;

internal static class WireRequestAssertions
{
    public static void AssertWireRequest(
        WireRequest request,
        string method,
        string path,
        string? query = null,
        string? bodyJson = null)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Path);
        Assert.Equal(query, request.Query);
        Assert.Equal(NormalizeJson(bodyJson), NormalizeJson(request.BodyJson));
    }

    private static string? NormalizeJson(string? json) =>
        json is null ? null : string.Concat(json.Where(c => !char.IsWhiteSpace(c)));
}
