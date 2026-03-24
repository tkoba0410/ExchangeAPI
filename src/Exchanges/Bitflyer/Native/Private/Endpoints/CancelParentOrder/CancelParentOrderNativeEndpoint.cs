using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;

public interface ICancelParentOrderNativeEndpoint
{
    Task<Call<CancelParentOrderRequest, Unit>> CallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class CancelParentOrderNativeEndpoint : ICancelParentOrderNativeEndpoint
{
    private readonly ICancelParentOrderProtocolEndpoint _protocolEndpoint;

    public CancelParentOrderNativeEndpoint(ICancelParentOrderProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<CancelParentOrderRequest, Unit>> CallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
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

        var protocolCall = await _protocolEndpoint.SendAsync(JsonSerializer.Serialize(request), cancellationToken);
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
