using System;
using System.Net;
using System.Text.Json;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal;

internal static class BittradeRawJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static bool TryDeserialize<T>(string json, out T? value, out Exception? error)
    {
        try
        {
            value = JsonSerializer.Deserialize<T>(json, Options);
            if (value is null)
            {
                error = new JsonException($"Failed to deserialize response as {typeof(T).Name}.");
                return false;
            }

            error = null;
            return true;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            value = default;
            error = ex;
            return false;
        }
    }

    public static bool TrySerialize<T>(T value, out string? json, out Exception? error)
    {
        if (value is null)
        {
            json = null;
            error = new ArgumentNullException(nameof(value));
            return false;
        }

        try
        {
            json = JsonSerializer.Serialize(value, Options);
            error = null;
            return true;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            json = null;
            error = ex;
            return false;
        }
    }

    public static T DeserializeOrThrow<T>(string json, string context)
    {
        if (TryDeserialize<T>(json, out var value, out var error))
        {
            return value!;
        }

        throw new JsonException(
            $"Failed to deserialize {context}.",
            error);
    }

    public static string SerializeOrThrow<T>(T value, string context)
    {
        if (TrySerialize(value, out var json, out var error))
        {
            return json!;
        }

        throw new JsonException(
            $"Failed to serialize {context}.",
            error);
    }

    public static TransportException CreateStatusException(string context, int statusCode, string json)
    {
        var payload = string.IsNullOrEmpty(json)
            ? "<empty>"
            : json.Length > 512
                ? json[..512] + "..."
                : json;

        return new TransportException(
            $"{context} failed with status {statusCode}. Payload: {payload}",
            statusCode: ToStatusCode(statusCode));
    }

    private static HttpStatusCode? ToStatusCode(int status) =>
        Enum.IsDefined(typeof(HttpStatusCode), status) ? (HttpStatusCode)status : null;
}
