using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;

public interface IGetWithdrawalsNativeEndpoint
{
    Task<Call<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> CallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetWithdrawalsNativeEndpoint : IGetWithdrawalsNativeEndpoint
{
    private readonly IGetWithdrawalsProtocolEndpoint _protocolEndpoint;

    public GetWithdrawalsNativeEndpoint(IGetWithdrawalsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> CallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetWithdrawals,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(
            request.Count,
            request.Before,
            request.After,
            request.MessageId,
            cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetWithdrawals,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetWithdrawals,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetWithdrawals.Item>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetWithdrawals.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    OrderId = JsonValueReader.ReadRequiredString(item, "order_id"),
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    Amount = JsonValueReader.ReadRequiredDecimal(item, "amount"),
                    Status = JsonValueReader.ReadRequiredString(item, "status"),
                    EventDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "event_date"),
                });
            }

            return NativeCallFactory.Success<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetWithdrawals,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetWithdrawalsRequest request)
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

        if (request.MessageId is not null && string.IsNullOrWhiteSpace(request.MessageId))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "MessageId must not be blank." };
        }

        return null;
    }
}
