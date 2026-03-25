using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;

public interface IGetCollateralHistoryNativeEndpoint
{
    Task<Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> CallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetCollateralHistoryNativeEndpoint : IGetCollateralHistoryNativeEndpoint
{
    private readonly IGetCollateralHistoryProtocolEndpoint _protocolEndpoint;

    public GetCollateralHistoryNativeEndpoint(IGetCollateralHistoryProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> CallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetCollateralHistory,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.Count, request.Before, request.After, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetCollateralHistory,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetCollateralHistory,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetCollateralHistory.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetCollateralHistory.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    Change = JsonValueReader.ReadRequiredDecimal(item, "change"),
                    Amount = JsonValueReader.ReadRequiredDecimal(item, "amount"),
                    ReasonCode = JsonValueReader.ReadRequiredString(item, "reason_code"),
                    Date = JsonValueReader.ReadRequiredUtcTimestamp(item, "date"),
                });
            }

            return NativeCallFactory.Success<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetCollateralHistory,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetCollateralHistoryRequest request)
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
