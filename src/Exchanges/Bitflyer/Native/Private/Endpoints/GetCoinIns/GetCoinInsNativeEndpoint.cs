using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;

public interface IGetCoinInsNativeEndpoint
{
    Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> CallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> CallAsync(
        GetCoinInsRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class GetCoinInsNativeEndpoint : IGetCoinInsNativeEndpoint
{
    private readonly IGetCoinInsProtocolEndpoint _protocolEndpoint;

    public GetCoinInsNativeEndpoint(IGetCoinInsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> CallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> CallAsync(
        GetCoinInsRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> CallAsyncCore(
        GetCoinInsRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetCoinIns,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(request.Count, request.Before, request.After, cancellationToken)
            : _protocolEndpoint.SendAsync(request.Count, request.Before, request.After, credentialSession, cancellationToken));
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetCoinIns,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetCoinIns,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetCoinIns.Item>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetCoinIns.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    OrderId = JsonValueReader.ReadRequiredString(item, "order_id"),
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    Amount = JsonValueReader.ReadRequiredDecimal(item, "amount"),
                    Address = JsonValueReader.ReadRequiredString(item, "address"),
                    TxHash = JsonValueReader.ReadRequiredString(item, "tx_hash"),
                    Status = JsonValueReader.ReadRequiredEnum<BitflyerTransferStatus>(item, "status"),
                    EventDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "event_date"),
                });
            }

            return NativeCallFactory.Success<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetCoinIns,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetCoinInsRequest request)
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

        return null;
    }
}
