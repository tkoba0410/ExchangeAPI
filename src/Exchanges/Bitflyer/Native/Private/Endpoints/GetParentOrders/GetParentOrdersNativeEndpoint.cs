using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;

public interface IGetParentOrdersNativeEndpoint
{
    Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> CallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> CallAsync(
        GetParentOrdersRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class GetParentOrdersNativeEndpoint : IGetParentOrdersNativeEndpoint
{
    private readonly IGetParentOrdersProtocolEndpoint _protocolEndpoint;

    public GetParentOrdersNativeEndpoint(IGetParentOrdersProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> CallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> CallAsync(
        GetParentOrdersRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> CallAsyncCore(
        GetParentOrdersRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetParentOrders,
                scope: "Private",
                auth: "KeySecret");
        }

        var parentOrderStateText = request.ParentOrderState is { } parentOrderState
            ? ApiStringEnum<BitflyerOrderState>.Format(parentOrderState)
            : null;
        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(request.ProductCode, request.Count, request.Before, request.After, parentOrderStateText, cancellationToken)
            : _protocolEndpoint.SendAsync(request.ProductCode, request.Count, request.Before, request.After, parentOrderStateText, credentialSession, cancellationToken));
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetParentOrders,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetParentOrders,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetParentOrders.Item>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetParentOrders.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    ParentOrderId = JsonValueReader.ReadRequiredString(item, "parent_order_id"),
                    ProductCode = JsonValueReader.ReadRequiredString(item, "product_code"),
                    Side = JsonValueReader.ReadRequiredEnum<BitflyerParentOrderSide>(item, "side"),
                    ParentOrderType = JsonValueReader.ReadRequiredEnum<BitflyerParentOrderType>(item, "parent_order_type"),
                    Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                    AveragePrice = JsonValueReader.ReadRequiredDecimal(item, "average_price"),
                    Size = JsonValueReader.ReadRequiredDecimal(item, "size"),
                    ParentOrderState = JsonValueReader.ReadRequiredEnum<BitflyerOrderState>(item, "parent_order_state"),
                    ExpireDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "expire_date"),
                    ParentOrderDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "parent_order_date"),
                    ParentOrderAcceptanceId = JsonValueReader.ReadRequiredString(item, "parent_order_acceptance_id"),
                    OutstandingSize = JsonValueReader.ReadRequiredDecimal(item, "outstanding_size"),
                    CancelSize = JsonValueReader.ReadRequiredDecimal(item, "cancel_size"),
                    ExecutedSize = JsonValueReader.ReadRequiredDecimal(item, "executed_size"),
                    TotalCommission = JsonValueReader.ReadRequiredDecimal(item, "total_commission"),
                });
            }

            return NativeCallFactory.Success<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetParentOrders,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetParentOrdersRequest request)
    {
        if (request.ProductCode is not null && string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode must not be blank." };
        }

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

        if (request.ParentOrderState is not null &&
            !ApiStringEnum<BitflyerOrderState>.IsDefined(request.ParentOrderState.Value))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "ParentOrderState must be ACTIVE, COMPLETED, CANCELED, EXPIRED or REJECTED.",
            };
        }

        return null;
    }
}
