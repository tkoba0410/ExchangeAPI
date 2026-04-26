using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;

public interface ICancelChildOrderNativeEndpoint
{
    Task<CallResult<CancelChildOrderRequest, Unit>> CallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<CancelChildOrderRequest, Unit>> CallAsync(
        CancelChildOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class CancelChildOrderNativeEndpoint : ICancelChildOrderNativeEndpoint
{
    private readonly ICancelChildOrderProtocolEndpoint _protocolEndpoint;

    public CancelChildOrderNativeEndpoint(ICancelChildOrderProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<CancelChildOrderRequest, Unit>> CallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<CancelChildOrderRequest, Unit>> CallAsync(
        CancelChildOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<CancelChildOrderRequest, Unit>> CallAsyncCore(
        CancelChildOrderRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<CancelChildOrderRequest, Unit>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.CancelChildOrder,
                scope: "Private",
                auth: "KeySecret");
        }

        var body = JsonSerializer.Serialize(request);
        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(body, cancellationToken)
            : _protocolEndpoint.SendAsync(body, credentialSession, cancellationToken));
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<CancelChildOrderRequest, Unit>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.CancelChildOrder,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<CancelChildOrderRequest, Unit>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.CancelChildOrder,
                "Private",
                "KeySecret");
        }

        if (string.IsNullOrWhiteSpace(protocolCall.Response.BodyText))
        {
            return NativeCallFactory.Success(request, new Unit(), protocolCall, "Private");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new CodecException("Expected top-level object.");
            }

            return NativeCallFactory.Success(request, new Unit(), protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<CancelChildOrderRequest, Unit>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.CancelChildOrder,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(CancelChildOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode is required." };
        }

        var hasOrderId = !string.IsNullOrWhiteSpace(request.ChildOrderId);
        var hasAcceptanceId = !string.IsNullOrWhiteSpace(request.ChildOrderAcceptanceId);
        if (hasOrderId == hasAcceptanceId)
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "Exactly one of ChildOrderId or ChildOrderAcceptanceId is required.",
            };
        }

        return null;
    }
}
