using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;

public interface ISendChildOrderNativeEndpoint
{
    Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(
        SendChildOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class SendChildOrderNativeEndpoint : ISendChildOrderNativeEndpoint
{
    private readonly ISendChildOrderProtocolEndpoint _protocolEndpoint;

    public SendChildOrderNativeEndpoint(ISendChildOrderProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(
        SendChildOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> CallAsyncCore(
        SendChildOrderRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
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
        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(body, cancellationToken)
            : _protocolEndpoint.SendAsync(body, credentialSession, cancellationToken));
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

        if (!ApiStringEnum<BitflyerChildOrderType>.IsDefined(request.ChildOrderType))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "ChildOrderType must be LIMIT or MARKET.",
            };
        }

        if (!ApiStringEnum<BitflyerOrderSide>.IsDefined(request.Side))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "Side must be BUY or SELL.",
            };
        }

        if (request.Size <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Size must be greater than zero." };
        }

        if (request.MinuteToExpire is not null && (request.MinuteToExpire <= 0 || request.MinuteToExpire > 43200))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "MinuteToExpire must be between 1 and 43200.",
            };
        }

        if (request.TimeInForce is not null &&
            !ApiStringEnum<BitflyerTimeInForce>.IsDefined(request.TimeInForce.Value))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "TimeInForce must be GTC, IOC or FOK.",
            };
        }

        if (request.ChildOrderType == ChildOrderTypes.Limit)
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

        if (request.ChildOrderType == ChildOrderTypes.Market && request.Price is not null)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Price must be omitted for MARKET orders." };
        }

        return null;
    }

    private static string BuildBody(SendChildOrderRequest request)
    {
        var normalizedRequest = request;
        if (request.ChildOrderType == ChildOrderTypes.Market)
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
