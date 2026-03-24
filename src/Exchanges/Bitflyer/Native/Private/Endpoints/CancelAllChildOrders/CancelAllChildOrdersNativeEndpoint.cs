using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;

public interface ICancelAllChildOrdersNativeEndpoint
{
    Task<Call<CancelAllChildOrdersRequest, Unit>> CallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class CancelAllChildOrdersNativeEndpoint : ICancelAllChildOrdersNativeEndpoint
{
    private readonly ICancelAllChildOrdersProtocolEndpoint _protocolEndpoint;

    public CancelAllChildOrdersNativeEndpoint(ICancelAllChildOrdersProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<CancelAllChildOrdersRequest, Unit>> CallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
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

        var bodyJson = JsonSerializer.Serialize(new Dictionary<string, string>
        {
            ["product_code"] = request.ProductCode,
        });

        var protocolCall = await _protocolEndpoint.SendAsync(bodyJson, cancellationToken);
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
