using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;

public interface IGetExecutionsPublicNativeEndpoint
{
    Task<Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> CallAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetExecutionsPublicNativeEndpoint : IGetExecutionsPublicNativeEndpoint
{
    private readonly IGetExecutionsPublicProtocolEndpoint _protocolEndpoint;

    public GetExecutionsPublicNativeEndpoint(IGetExecutionsPublicProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> CallAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetExecutionsPublic,
                scope: "Public",
                auth: "None");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After,
            cancellationToken);

        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetExecutionsPublic,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetExecutionsPublic,
                "Public",
                "None");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetExecutionsPublic.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetExecutionsPublic.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    Side = JsonValueReader.ReadRequiredString(item, "side"),
                    Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                    Size = JsonValueReader.ReadRequiredDecimal(item, "size"),
                    ExecDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "exec_date"),
                    BuyChildOrderAcceptanceId = JsonValueReader.ReadRequiredString(item, "buy_child_order_acceptance_id"),
                    SellChildOrderAcceptanceId = JsonValueReader.ReadRequiredString(item, "sell_child_order_acceptance_id"),
                });
            }

            return NativeCallFactory.Success<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>(request, items, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetExecutionsPublic,
                "Public",
                "None");
        }
    }

    private static CallError? Validate(GetExecutionsPublicRequest request)
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

        return null;
    }
}
