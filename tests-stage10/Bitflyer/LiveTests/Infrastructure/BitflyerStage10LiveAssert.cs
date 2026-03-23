using System.Globalization;
using System.Text.Json;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;
using Xunit.Sdk;

namespace ExchangeApi.Tests.Stage10.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerStage10LiveAssert
{
    public static TResponse RequireOk<TRequest, TResponse>(Call<TRequest, TResponse> call) =>
        call.Result switch
        {
            CallResult<TResponse>.Ok ok => ok.Response,
            CallResult<TResponse>.Err err => throw new XunitException(
                $"Live call failed. endpoint={call.Meta.EndpointId}, layer={call.Meta.Layer}, component={call.Meta.Component}, " +
                $"kind={err.Error.Kind}, http={err.Error.HttpStatus}, message={err.Error.Message}, body={Truncate(err.Error.BodySnippet)}"),
            _ => throw new XunitException("Unexpected call result type.")
        };

    public static WireResponse RequireWireSuccess(
        Call<WireCallSpec, WireResponse> call,
        bool requireJsonBody = true)
    {
        var response = RequireOk(call);
        if (response.StatusCode != 200)
        {
            throw new XunitException(
                $"Expected HTTP 200. endpoint={call.Request.EndpointId}, status={response.StatusCode}, body={Truncate(response.Json)}");
        }

        if (requireJsonBody && string.IsNullOrWhiteSpace(response.Json))
        {
            throw new XunitException($"Expected a JSON payload for endpoint={call.Request.EndpointId}.");
        }

        return response;
    }

    public static DateTimeOffset ParseTimestamp(JsonElement property)
    {
        var value = property.GetString();
        if (value is not null &&
            DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            return parsed;
        }

        throw new XunitException($"Timestamp '{value}' was not a valid ISO-8601 value.");
    }

    private static string Truncate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Length <= 512 ? value : value[..512];
    }
}
