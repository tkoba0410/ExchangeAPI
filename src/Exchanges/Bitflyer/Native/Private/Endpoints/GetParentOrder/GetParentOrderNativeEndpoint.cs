using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;

public interface IGetParentOrderNativeEndpoint
{
    Task<Call<GetParentOrderRequest, GetParentOrderResponse>> CallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetParentOrderNativeEndpoint : IGetParentOrderNativeEndpoint
{
    private readonly IGetParentOrderProtocolEndpoint _protocolEndpoint;

    public GetParentOrderNativeEndpoint(IGetParentOrderProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetParentOrderRequest, GetParentOrderResponse>> CallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetParentOrderRequest, GetParentOrderResponse>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetParentOrder,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(
            request.ParentOrderId,
            request.ParentOrderAcceptanceId,
            cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetParentOrderRequest, GetParentOrderResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetParentOrder,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetParentOrderRequest, GetParentOrderResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetParentOrder,
                "Private",
                "KeySecret");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            if (!root.TryGetProperty("parameters", out var parametersElement) ||
                parametersElement.ValueKind != System.Text.Json.JsonValueKind.Array)
            {
                throw new CodecException("Property 'parameters' must be an array.");
            }

            var parameters = new List<GetParentOrderParameter>();
            foreach (var parameter in parametersElement.EnumerateArray())
            {
                if (parameter.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Parameter item must be an object.");
                }

                parameters.Add(new GetParentOrderParameter
                {
                    ProductCode = JsonValueReader.ReadRequiredString(parameter, "product_code"),
                    ConditionType = JsonValueReader.ReadRequiredString(parameter, "condition_type"),
                    Side = JsonValueReader.ReadRequiredString(parameter, "side"),
                    Price = JsonValueReader.ReadRequiredDecimal(parameter, "price"),
                    Size = JsonValueReader.ReadRequiredDecimal(parameter, "size"),
                    TriggerPrice = JsonValueReader.ReadRequiredDecimal(parameter, "trigger_price"),
                    Offset = JsonValueReader.ReadRequiredDecimal(parameter, "offset"),
                });
            }

            var response = new GetParentOrderResponse
            {
                Id = JsonValueReader.ReadRequiredLong(root, "id"),
                ParentOrderId = JsonValueReader.ReadRequiredString(root, "parent_order_id"),
                OrderMethod = JsonValueReader.ReadRequiredString(root, "order_method"),
                ExpireDate = JsonValueReader.ReadRequiredUtcTimestamp(root, "expire_date"),
                TimeInForce = JsonValueReader.ReadRequiredString(root, "time_in_force"),
                Parameters = parameters,
                ParentOrderAcceptanceId = JsonValueReader.ReadRequiredString(root, "parent_order_acceptance_id"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetParentOrderRequest, GetParentOrderResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetParentOrder,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetParentOrderRequest request)
    {
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
