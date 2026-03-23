using System.Text.Json;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Errors;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.JsonValidation;

internal static class ProtocolJsonValidator
{
    public static bool TryValidateExpectedStatus(
        WireResponse response,
        int expectedStatusCode,
        out CallError? error)
    {
        if (response.StatusCode == expectedStatusCode)
        {
            error = null;
            return true;
        }

        error = BitflyerErrorFactory.Http(
            $"HTTP status {response.StatusCode} returned from bitFlyer. Expected {expectedStatusCode}.",
            response);
        return false;
    }

    public static bool TryValidateObjectResponse(
        WireResponse response,
        out JsonValidationResult result,
        out CallError? error)
    {
        if (!TryValidateSuccessStatus(response, out result, out error))
        {
            return false;
        }

        if (result.Root.ValueKind != JsonValueKind.Object)
        {
            result.Document.Dispose();
            result = default;
            error = BitflyerErrorFactory.Codec("Response JSON must be an object.", response);
            return false;
        }

        return true;
    }

    public static bool TryValidateArrayResponse(
        WireResponse response,
        out JsonValidationResult result,
        out CallError? error)
    {
        if (!TryValidateSuccessStatus(response, out result, out error))
        {
            return false;
        }

        if (result.Root.ValueKind != JsonValueKind.Array)
        {
            result.Document.Dispose();
            result = default;
            error = BitflyerErrorFactory.Codec("Response JSON must be an array.", response);
            return false;
        }

        return true;
    }

    private static bool TryValidateSuccessStatus(
        WireResponse response,
        out JsonValidationResult result,
        out CallError? error)
    {
        if (response.StatusCode < 200 || response.StatusCode >= 300)
        {
            result = default;
            error = BitflyerErrorFactory.Http(
                $"HTTP status {response.StatusCode} returned from bitFlyer.",
                response);
            return false;
        }

        try
        {
            var document = JsonDocument.Parse(response.Json);
            result = new JsonValidationResult(document, document.RootElement);
            error = null;
            return true;
        }
        catch (JsonException ex)
        {
            result = default;
            error = BitflyerErrorFactory.Codec("Response JSON could not be parsed.", response, ex);
            return false;
        }
    }
}
