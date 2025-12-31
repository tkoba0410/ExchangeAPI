using System;
using System.Net;
using System.Text.Json;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

internal static class BitflyerRawJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static T ParseOrThrow<T>(WireResponse response)
    {
        EnsureSuccess(response);

        try
        {
            var result = JsonSerializer.Deserialize<T>(response.Json, Options);
            if (result is null)
            {
                throw new JsonException($"Failed to deserialize response as {typeof(T).Name}.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            throw new TransportException(
                "Failed to deserialize JSON response.",
                statusCode: ToStatusCode(response.StatusCode),
                innerException: ex);
        }
    }

    private static void EnsureSuccess(WireResponse response)
    {
        if (response.StatusCode is < 200 or >= 300)
        {
            throw new TransportException(
                $"Request failed with status {response.StatusCode}.",
                statusCode: ToStatusCode(response.StatusCode));
        }
    }

    private static HttpStatusCode? ToStatusCode(int status) =>
        Enum.IsDefined(typeof(HttpStatusCode), status) ? (HttpStatusCode)status : null;
}
