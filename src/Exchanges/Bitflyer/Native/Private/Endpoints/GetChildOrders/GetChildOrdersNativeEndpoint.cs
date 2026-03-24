using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;

public interface IGetChildOrdersNativeEndpoint
{
    Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> CallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetChildOrdersNativeEndpoint : IGetChildOrdersNativeEndpoint
{
    private readonly IGetChildOrdersProtocolEndpoint _protocolEndpoint;

    public GetChildOrdersNativeEndpoint(IGetChildOrdersProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> CallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetChildOrders,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After,
            request.ChildOrderState,
            request.ChildOrderId,
            request.ChildOrderAcceptanceId,
            request.ParentOrderId,
            cancellationToken);

        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetChildOrders,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetChildOrders,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetChildOrders.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetChildOrders.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    ChildOrderId = JsonValueReader.ReadRequiredString(item, "child_order_id"),
                    ProductCode = JsonValueReader.ReadRequiredString(item, "product_code"),
                    Side = JsonValueReader.ReadRequiredString(item, "side"),
                    ChildOrderType = JsonValueReader.ReadRequiredString(item, "child_order_type"),
                    Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                    AveragePrice = JsonValueReader.ReadRequiredDecimal(item, "average_price"),
                    Size = JsonValueReader.ReadRequiredDecimal(item, "size"),
                    ChildOrderState = JsonValueReader.ReadRequiredString(item, "child_order_state"),
                    ExpireDate = JsonValueReader.ReadRequiredTimestamp(item, "expire_date"),
                    ChildOrderDate = JsonValueReader.ReadRequiredTimestamp(item, "child_order_date"),
                    ChildOrderAcceptanceId = JsonValueReader.ReadRequiredString(item, "child_order_acceptance_id"),
                    OutstandingSize = JsonValueReader.ReadRequiredDecimal(item, "outstanding_size"),
                    CancelSize = JsonValueReader.ReadRequiredDecimal(item, "cancel_size"),
                    ExecutedSize = JsonValueReader.ReadRequiredDecimal(item, "executed_size"),
                    TotalCommission = JsonValueReader.ReadRequiredDecimal(item, "total_commission"),
                    TimeInForce = JsonValueReader.ReadRequiredString(item, "time_in_force"),
                });
            }

            return NativeCallFactory.Success<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetChildOrders,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetChildOrdersRequest request)
    {
        if (request.Count is not null && request.Count <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Count must be greater than zero." };
        }

        if (request.Before is not null && request.Before <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Before must be greater than zero." };
        }

        if (request.After is not null && request.After <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "After must be greater than zero." };
        }

        if (!string.IsNullOrWhiteSpace(request.ChildOrderState) &&
            !string.Equals(request.ChildOrderState, ChildOrderStates.Active, StringComparison.Ordinal) &&
            !string.Equals(request.ChildOrderState, ChildOrderStates.Completed, StringComparison.Ordinal) &&
            !string.Equals(request.ChildOrderState, ChildOrderStates.Canceled, StringComparison.Ordinal) &&
            !string.Equals(request.ChildOrderState, ChildOrderStates.Expired, StringComparison.Ordinal) &&
            !string.Equals(request.ChildOrderState, ChildOrderStates.Rejected, StringComparison.Ordinal))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "ChildOrderState must be ACTIVE, COMPLETED, CANCELED, EXPIRED or REJECTED.",
            };
        }

        return null;
    }
}
