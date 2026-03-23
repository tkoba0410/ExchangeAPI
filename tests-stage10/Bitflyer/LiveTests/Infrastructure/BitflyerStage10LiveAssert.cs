using System.Globalization;
using System.Text.Json;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;
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

    public static string ParseAcceptanceId(JsonElement root)
    {
        if (!root.TryGetProperty("child_order_acceptance_id", out var property))
        {
            throw new XunitException("Response JSON did not contain child_order_acceptance_id.");
        }

        var value = property.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new XunitException("child_order_acceptance_id was empty.");
        }

        return value;
    }

    public static async Task CancelChildOrderWithRetryAsync(
        IBitflyerPrivateNativeApi api,
        string productCode,
        string acceptanceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(api);
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(acceptanceId);

        var lastError = string.Empty;
        for (var attempt = 1; attempt <= 5; attempt++)
        {
            var call = await api.CancelChildOrderCallAsync(
                new CancelChildOrderRequest
                {
                    ProductCode = productCode,
                    ChildOrderAcceptanceId = acceptanceId,
                },
                cancellationToken).ConfigureAwait(false);

            if (call.Result is CallResult<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.CancelChildOrderResponse>.Ok)
            {
                return;
            }

            var err = (CallResult<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.CancelChildOrderResponse>.Err)call.Result;
            lastError = $"kind={err.Error.Kind}, http={err.Error.HttpStatus}, message={err.Error.Message}, body={Truncate(err.Error.BodySnippet)}";
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
        }

        throw new XunitException($"Failed to cancel child order after retry. acceptanceId={acceptanceId}, {lastError}");
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
