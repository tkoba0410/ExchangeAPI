using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;

public interface ISendChildOrderNativeEndpoint
{
    Task<Call<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class SendChildOrderNativeEndpoint : ISendChildOrderNativeEndpoint
{
    private readonly ISendChildOrderProtocolEndpoint _protocolEndpoint;

    public SendChildOrderNativeEndpoint(ISendChildOrderProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<SendChildOrderRequest, SendChildOrderResponse>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.SendChildOrder,
                scope: "Private",
                auth: "KeySecret");
        }

        var body = BuildBody(request);
        var protocolCall = await _protocolEndpoint.SendAsync(body, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<SendChildOrderRequest, SendChildOrderResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.SendChildOrder,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<SendChildOrderRequest, SendChildOrderResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.SendChildOrder,
                "Private",
                "KeySecret");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new SendChildOrderResponse
            {
                ChildOrderAcceptanceId = JsonValueReader.ReadRequiredString(root, "child_order_acceptance_id"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<SendChildOrderRequest, SendChildOrderResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.SendChildOrder,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(SendChildOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode is required." };
        }

        if (string.IsNullOrWhiteSpace(request.ChildOrderType))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "ChildOrderType is required." };
        }

        if (string.IsNullOrWhiteSpace(request.Side))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Side is required." };
        }

        if (request.Size <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Size must be greater than zero." };
        }

        if (string.Equals(request.ChildOrderType, ChildOrderTypes.Limit, StringComparison.Ordinal))
        {
            if (request.Price is null)
            {
                return new CallError { Kind = CallErrorKinds.Semantic, Message = "Price is required for LIMIT orders." };
            }

            if (request.Price <= 0)
            {
                return new CallError { Kind = CallErrorKinds.Semantic, Message = "Price must be greater than zero." };
            }
        }

        if (string.Equals(request.ChildOrderType, ChildOrderTypes.Market, StringComparison.Ordinal) && request.Price is not null)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Price must be omitted for MARKET orders." };
        }

        return null;
    }

    private static string BuildBody(SendChildOrderRequest request)
    {
        var normalizedRequest = request;
        if (string.Equals(request.ChildOrderType, ChildOrderTypes.Market, StringComparison.Ordinal))
        {
            normalizedRequest = new SendChildOrderRequest
            {
                ProductCode = request.ProductCode,
                ChildOrderType = request.ChildOrderType,
                Side = request.Side,
                Price = null,
                Size = request.Size,
                MinuteToExpire = request.MinuteToExpire,
                TimeInForce = request.TimeInForce,
            };
        }

        return JsonSerializer.Serialize(normalizedRequest);
    }
}
