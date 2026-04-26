using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;

public interface IGetExecutionsNativeEndpoint
{
    Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsync(
        GetExecutionsRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class GetExecutionsNativeEndpoint : IGetExecutionsNativeEndpoint
{
    private readonly IGetExecutionsProtocolEndpoint _protocolEndpoint;

    public GetExecutionsNativeEndpoint(IGetExecutionsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsync(
        GetExecutionsRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsyncCore(
        GetExecutionsRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetExecutionsPrivate,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(request.ProductCode, request.Count, request.Before, request.After, request.ChildOrderId, request.ChildOrderAcceptanceId, cancellationToken)
            : _protocolEndpoint.SendAsync(request.ProductCode, request.Count, request.Before, request.After, request.ChildOrderId, request.ChildOrderAcceptanceId, credentialSession, cancellationToken));

        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetExecutionsPrivate,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetExecutionsPrivate,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetExecutions.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetExecutions.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    ChildOrderId = JsonValueReader.ReadRequiredString(item, "child_order_id"),
                    Side = JsonValueReader.ReadRequiredEnum<BitflyerOrderSide>(item, "side"),
                    Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                    Size = JsonValueReader.ReadRequiredDecimal(item, "size"),
                    Commission = JsonValueReader.ReadRequiredDecimal(item, "commission"),
                    ExecDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "exec_date"),
                    ChildOrderAcceptanceId = JsonValueReader.ReadRequiredString(item, "child_order_acceptance_id"),
                });
            }

            return NativeCallFactory.Success<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetExecutionsPrivate,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetExecutionsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode is required." };
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

        return null;
    }
}
