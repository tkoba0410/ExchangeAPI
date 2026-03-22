using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Errors;
using ExchangeApi.Stage10.Bitflyer.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Encoder;

internal static class GetTickerRequestEncoder
{
    public static bool TryEncode(
        GetTickerRequest request,
        out EncodedGetTickerRequest encodedRequest,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ProductCode is not null && string.IsNullOrWhiteSpace(request.ProductCode))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("ProductCode must be null or a non-empty string.");
            return false;
        }

        encodedRequest = new EncodedGetTickerRequest(request.ProductCode);
        error = null;
        return true;
    }
}

internal readonly record struct EncodedGetTickerRequest(string? ProductCode);
