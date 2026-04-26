using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;

public interface ICancelAllChildOrdersNativeEndpoint
{
    Task<CallResult<CancelAllChildOrdersRequest, Unit>> CallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<CancelAllChildOrdersRequest, Unit>> CallAsync(
        CancelAllChildOrdersRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class CancelAllChildOrdersNativeEndpoint : ICancelAllChildOrdersNativeEndpoint
{
    private readonly ICancelAllChildOrdersProtocolEndpoint _protocolEndpoint;

    public CancelAllChildOrdersNativeEndpoint(ICancelAllChildOrdersProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<CancelAllChildOrdersRequest, Unit>> CallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<CancelAllChildOrdersRequest, Unit>> CallAsync(
        CancelAllChildOrdersRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<CancelAllChildOrdersRequest, Unit>> CallAsyncCore(
        CancelAllChildOrdersRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return NativeCallFactory.Failure<CancelAllChildOrdersRequest, Unit>(
                request,
                new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode is required." },
                protocolCall: null,
                endpointId: BitflyerEndpointIds.CancelAllChildOrders,
                scope: "Private",
                auth: "KeySecret");
        }

        var bodyJson = JsonSerializer.Serialize(request);

        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(bodyJson, cancellationToken)
            : _protocolEndpoint.SendAsync(bodyJson, credentialSession, cancellationToken));
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<CancelAllChildOrdersRequest, Unit>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.CancelAllChildOrders,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<CancelAllChildOrdersRequest, Unit>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.CancelAllChildOrders,
                "Private",
                "KeySecret");
        }

        return NativeCallFactory.Success(request, new Unit(), protocolCall, "Private");
    }
}
