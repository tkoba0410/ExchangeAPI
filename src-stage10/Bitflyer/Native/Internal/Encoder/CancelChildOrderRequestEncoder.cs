using System.Text.Json;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Encoder;

internal static class CancelChildOrderRequestEncoder
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static bool TryEncode(
        CancelChildOrderRequest request,
        out EncodedCancelChildOrderRequest encodedRequest,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("ProductCode is required.");
            return false;
        }

        if (request.ChildOrderId is not null && string.IsNullOrWhiteSpace(request.ChildOrderId))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("ChildOrderId must be null or a non-empty string.");
            return false;
        }

        if (request.ChildOrderAcceptanceId is not null && string.IsNullOrWhiteSpace(request.ChildOrderAcceptanceId))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("ChildOrderAcceptanceId must be null or a non-empty string.");
            return false;
        }

        var hasChildOrderId = !string.IsNullOrWhiteSpace(request.ChildOrderId);
        var hasChildOrderAcceptanceId = !string.IsNullOrWhiteSpace(request.ChildOrderAcceptanceId);
        if (hasChildOrderId == hasChildOrderAcceptanceId)
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic(
                "Exactly one of ChildOrderId or ChildOrderAcceptanceId must be specified.");
            return false;
        }

        try
        {
            var body = new CancelChildOrderRequestBody
            {
                ProductCode = request.ProductCode,
                ChildOrderId = request.ChildOrderId,
                ChildOrderAcceptanceId = request.ChildOrderAcceptanceId,
            };

            encodedRequest = new EncodedCancelChildOrderRequest(
                JsonSerializer.Serialize(body, SerializerOptions));
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            encodedRequest = default;
            error = new CallError(CallErrorKind.Codec, "Failed to serialize CancelChildOrder request.", ex);
            return false;
        }
    }

    private sealed class CancelChildOrderRequestBody
    {
        [JsonPropertyName("product_code")]
        public string ProductCode { get; init; } = string.Empty;

        [JsonPropertyName("child_order_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ChildOrderId { get; init; }

        [JsonPropertyName("child_order_acceptance_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ChildOrderAcceptanceId { get; init; }
    }
}

internal readonly record struct EncodedCancelChildOrderRequest(string BodyJson);
