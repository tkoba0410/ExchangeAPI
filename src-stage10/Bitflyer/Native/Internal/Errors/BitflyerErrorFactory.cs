using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Shared;

internal static class BitflyerErrorFactory
{
    public static CallError Codec(string message) =>
        new(
            Kind: CallErrorKind.Codec,
            Message: message);

    public static CallError Http(string message, WireResponse response) =>
        new(
            Kind: CallErrorKind.Http,
            Message: message,
            HttpStatus: response.StatusCode,
            BodySnippet: CreateBodySnippet(response.Json));

    public static CallError Codec(string message, WireResponse response, Exception? exception = null) =>
        new(
            Kind: CallErrorKind.Codec,
            Message: message,
            Exception: exception,
            HttpStatus: response.StatusCode,
            BodySnippet: CreateBodySnippet(response.Json));

    public static CallError Mapping(string message, Exception? exception = null) =>
        new(
            Kind: CallErrorKind.Mapping,
            Message: message,
            Exception: exception);

    public static CallError Semantic(string message) =>
        new(
            Kind: CallErrorKind.Semantic,
            Message: message);

    private static string? CreateBodySnippet(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        const int maxLength = 256;
        return json.Length <= maxLength
            ? json
            : json[..maxLength];
    }
}
