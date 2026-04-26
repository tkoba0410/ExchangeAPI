using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;

public interface ISendParentOrderNativeEndpoint
{
    Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> CallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> CallAsync(
        SendParentOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class SendParentOrderNativeEndpoint : ISendParentOrderNativeEndpoint
{
    private readonly ISendParentOrderProtocolEndpoint _protocolEndpoint;

    public SendParentOrderNativeEndpoint(ISendParentOrderProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> CallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> CallAsync(
        SendParentOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> CallAsyncCore(
        SendParentOrderRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<SendParentOrderRequest, SendParentOrderResponse>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.SendParentOrder,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(JsonSerializer.Serialize(request), cancellationToken)
            : _protocolEndpoint.SendAsync(JsonSerializer.Serialize(request), credentialSession, cancellationToken));
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<SendParentOrderRequest, SendParentOrderResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.SendParentOrder,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<SendParentOrderRequest, SendParentOrderResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.SendParentOrder,
                "Private",
                "KeySecret");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new SendParentOrderResponse
            {
                ParentOrderAcceptanceId = JsonValueReader.ReadRequiredString(root, "parent_order_acceptance_id"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<SendParentOrderRequest, SendParentOrderResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.SendParentOrder,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(SendParentOrderRequest request)
    {
        var orderMethod = request.OrderMethod ?? ParentOrderMethods.Simple;

        if (!ApiStringEnum<BitflyerOrderMethod>.IsDefined(orderMethod))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "OrderMethod must be SIMPLE, IFD, OCO or IFDOCO.",
            };
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

        if (request.Parameters.Count == 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameters must contain at least one item." };
        }

        var expectedCount = orderMethod switch
        {
            ParentOrderMethods.Simple => 1,
            ParentOrderMethods.Ifd => 2,
            ParentOrderMethods.Oco => 2,
            ParentOrderMethods.IfdOco => 3,
            _ => 0,
        };

        if (request.Parameters.Count != expectedCount)
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = $"Parameters must contain exactly {expectedCount} item(s) for {orderMethod}.",
            };
        }

        foreach (var parameter in request.Parameters)
        {
            var parameterError = ValidateParameter(parameter);
            if (parameterError is not null)
            {
                return parameterError;
            }
        }

        return null;
    }

    private static CallError? ValidateParameter(SendParentOrderParameter parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter.ProductCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.ProductCode is required." };
        }

        if (!ApiStringEnum<BitflyerConditionType>.IsDefined(parameter.ConditionType))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "Parameter.ConditionType must be LIMIT, MARKET, STOP, STOP_LIMIT or TRAIL.",
            };
        }

        if (!ApiStringEnum<BitflyerOrderSide>.IsDefined(parameter.Side))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.Side must be BUY or SELL." };
        }

        if (parameter.Size <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.Size must be greater than zero." };
        }

        return parameter.ConditionType switch
        {
            ParentOrderConditionTypes.Limit => ValidateLimit(parameter),
            ParentOrderConditionTypes.Market => ValidateMarket(parameter),
            ParentOrderConditionTypes.Stop => ValidateStop(parameter),
            ParentOrderConditionTypes.StopLimit => ValidateStopLimit(parameter),
            ParentOrderConditionTypes.Trail => ValidateTrail(parameter),
            _ => null,
        };
    }

    private static CallError? ValidateLimit(SendParentOrderParameter parameter)
    {
        if (parameter.Price is null || parameter.Price <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.Price is required for LIMIT." };
        }

        if (parameter.TriggerPrice is not null || parameter.Offset is not null)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "LIMIT must omit TriggerPrice and Offset." };
        }

        return null;
    }

    private static CallError? ValidateMarket(SendParentOrderParameter parameter)
    {
        if (parameter.Price is not null || parameter.TriggerPrice is not null || parameter.Offset is not null)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "MARKET must omit Price, TriggerPrice and Offset." };
        }

        return null;
    }

    private static CallError? ValidateStop(SendParentOrderParameter parameter)
    {
        if (parameter.TriggerPrice is null || parameter.TriggerPrice <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.TriggerPrice is required for STOP." };
        }

        if (parameter.Price is not null || parameter.Offset is not null)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "STOP must omit Price and Offset." };
        }

        return null;
    }

    private static CallError? ValidateStopLimit(SendParentOrderParameter parameter)
    {
        if (parameter.Price is null || parameter.Price <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.Price is required for STOP_LIMIT." };
        }

        if (parameter.TriggerPrice is null || parameter.TriggerPrice <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.TriggerPrice is required for STOP_LIMIT." };
        }

        if (parameter.Offset is not null)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "STOP_LIMIT must omit Offset." };
        }

        return null;
    }

    private static CallError? ValidateTrail(SendParentOrderParameter parameter)
    {
        if (parameter.Offset is null || parameter.Offset <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Parameter.Offset is required for TRAIL." };
        }

        if (parameter.Price is not null || parameter.TriggerPrice is not null)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "TRAIL must omit Price and TriggerPrice." };
        }

        return null;
    }
}
