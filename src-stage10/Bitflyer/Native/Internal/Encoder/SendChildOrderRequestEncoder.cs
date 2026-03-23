using System.Text.Json;
using System.Text.Json.Serialization;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Errors;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Encoder;

internal static class SendChildOrderRequestEncoder
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static bool TryEncode(
        SendChildOrderRequest request,
        out EncodedSendChildOrderRequest encodedRequest,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("ProductCode is required.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.ChildOrderType))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("ChildOrderType is required.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Side))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("Side is required.");
            return false;
        }

        if (request.Size <= 0m)
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("Size must be greater than zero.");
            return false;
        }

        var childOrderType = request.ChildOrderType.ToUpperInvariant();
        if (childOrderType is not ("LIMIT" or "MARKET"))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("ChildOrderType must be LIMIT or MARKET.");
            return false;
        }

        var side = request.Side.ToUpperInvariant();
        if (side is not ("BUY" or "SELL"))
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("Side must be BUY or SELL.");
            return false;
        }

        if (childOrderType == "LIMIT")
        {
            if (request.Price is null)
            {
                encodedRequest = default;
                error = BitflyerErrorFactory.Semantic("Price is required when ChildOrderType is LIMIT.");
                return false;
            }

            if (request.Price <= 0m)
            {
                encodedRequest = default;
                error = BitflyerErrorFactory.Semantic("Price must be greater than zero when specified.");
                return false;
            }
        }
        else if (request.Price is not null)
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("Price must be omitted when ChildOrderType is MARKET.");
            return false;
        }

        if (request.MinuteToExpire is <= 0)
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("MinuteToExpire must be greater than zero when specified.");
            return false;
        }

        if (request.MinuteToExpire is > 43200)
        {
            encodedRequest = default;
            error = BitflyerErrorFactory.Semantic("MinuteToExpire must be less than or equal to 43200 when specified.");
            return false;
        }

        if (request.TimeInForce is not null)
        {
            if (string.IsNullOrWhiteSpace(request.TimeInForce))
            {
                encodedRequest = default;
                error = BitflyerErrorFactory.Semantic("TimeInForce must be null or a non-empty string.");
                return false;
            }

            var timeInForce = request.TimeInForce.ToUpperInvariant();
            if (timeInForce is not ("GTC" or "IOC" or "FOK"))
            {
                encodedRequest = default;
                error = BitflyerErrorFactory.Semantic("TimeInForce must be GTC, IOC, or FOK when specified.");
                return false;
            }
        }

        try
        {
            var body = new SendChildOrderRequestBody
            {
                ProductCode = request.ProductCode,
                ChildOrderType = childOrderType,
                Side = side,
                Size = request.Size,
                Price = request.Price,
                MinuteToExpire = request.MinuteToExpire,
                TimeInForce = request.TimeInForce?.ToUpperInvariant(),
            };

            encodedRequest = new EncodedSendChildOrderRequest(
                JsonSerializer.Serialize(body, SerializerOptions));
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            encodedRequest = default;
            error = new CallError(CallErrorKind.Codec, "Failed to serialize SendChildOrder request.", ex);
            return false;
        }
    }

    private sealed class SendChildOrderRequestBody
    {
        [JsonPropertyName("product_code")]
        public string ProductCode { get; init; } = string.Empty;

        [JsonPropertyName("child_order_type")]
        public string ChildOrderType { get; init; } = string.Empty;

        [JsonPropertyName("side")]
        public string Side { get; init; } = string.Empty;

        [JsonPropertyName("size")]
        public decimal Size { get; init; }

        [JsonPropertyName("price")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Price { get; init; }

        [JsonPropertyName("minute_to_expire")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinuteToExpire { get; init; }

        [JsonPropertyName("time_in_force")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TimeInForce { get; init; }
    }
}

internal readonly record struct EncodedSendChildOrderRequest(string BodyJson);
