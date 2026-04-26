using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;

public interface ICancelParentOrderNativeEndpoint
{
    Task<CallResult<CancelParentOrderRequest, Unit>> CallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<CancelParentOrderRequest, Unit>> CallAsync(
        CancelParentOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class CancelParentOrderNativeEndpoint : ICancelParentOrderNativeEndpoint
{
    private readonly ICancelParentOrderProtocolEndpoint _protocolEndpoint;

    public CancelParentOrderNativeEndpoint(ICancelParentOrderProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<CancelParentOrderRequest, Unit>> CallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<CancelParentOrderRequest, Unit>> CallAsync(
        CancelParentOrderRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<CancelParentOrderRequest, Unit>> CallAsyncCore(
        CancelParentOrderRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<CancelParentOrderRequest, Unit>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.CancelParentOrder,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(JsonSerializer.Serialize(request), cancellationToken)
            : _protocolEndpoint.SendAsync(JsonSerializer.Serialize(request), credentialSession, cancellationToken));
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<CancelParentOrderRequest, Unit>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.CancelParentOrder,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<CancelParentOrderRequest, Unit>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.CancelParentOrder,
                "Private",
                "KeySecret");
        }

        return NativeCallFactory.Success(request, new Unit(), protocolCall, "Private");
    }

    private static CallError? Validate(CancelParentOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode is required." };
        }

        var hasParentOrderId = !string.IsNullOrWhiteSpace(request.ParentOrderId);
        var hasAcceptanceId = !string.IsNullOrWhiteSpace(request.ParentOrderAcceptanceId);

        if (hasParentOrderId == hasAcceptanceId)
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "Exactly one of ParentOrderId or ParentOrderAcceptanceId must be specified.",
            };
        }

        return null;
    }
}
