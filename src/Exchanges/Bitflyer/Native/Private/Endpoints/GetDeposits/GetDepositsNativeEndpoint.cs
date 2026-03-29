using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;

public interface IGetDepositsNativeEndpoint
{
    Task<Call<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> CallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetDepositsNativeEndpoint : IGetDepositsNativeEndpoint
{
    private readonly IGetDepositsProtocolEndpoint _protocolEndpoint;

    public GetDepositsNativeEndpoint(IGetDepositsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> CallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetDeposits,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.Count, request.Before, request.After, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetDeposits,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetDeposits,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetDeposits.Item>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetDeposits.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    OrderId = JsonValueReader.ReadRequiredString(item, "order_id"),
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    Amount = JsonValueReader.ReadRequiredDecimal(item, "amount"),
                    Status = JsonValueReader.ReadRequiredEnum<BitflyerTransferStatus>(item, "status"),
                    EventDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "event_date"),
                });
            }

            return NativeCallFactory.Success<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetDeposits,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetDepositsRequest request)
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
