using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;

public interface IGetBoardNativeEndpoint
{
    Task<CallResult<GetBoardRequest, GetBoardResponse>> CallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetBoardNativeEndpoint : IGetBoardNativeEndpoint
{
    private readonly IGetBoardProtocolEndpoint _protocolEndpoint;

    public GetBoardNativeEndpoint(IGetBoardProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<CallResult<GetBoardRequest, GetBoardResponse>> CallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ProductCode is not null && string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return NativeCallFactory.Failure<GetBoardRequest, GetBoardResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode must not be blank." },
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetBoard,
                scope: "Public",
                auth: "None");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.ProductCode, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetBoardRequest, GetBoardResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetBoard,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetBoardRequest, GetBoardResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetBoard,
                "Public",
                "None");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new GetBoardResponse
            {
                MidPrice = JsonValueReader.ReadRequiredDecimal(root, "mid_price"),
                Bids = ReadLevels(root, "bids"),
                Asks = ReadLevels(root, "asks"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetBoardRequest, GetBoardResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetBoard,
                "Public",
                "None");
        }
    }

    private static IReadOnlyList<GetBoardLevel> ReadLevels(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property))
        {
            throw new CodecException($"Missing required property '{propertyName}'.");
        }

        if (property.ValueKind != JsonValueKind.Array)
        {
            throw new CodecException($"Property '{propertyName}' must be an array.");
        }

        var items = new List<GetBoardLevel>();
        foreach (var item in property.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                throw new CodecException($"Property '{propertyName}' array item must be an object.");
            }

            items.Add(new GetBoardLevel
            {
                Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                Size = JsonValueReader.ReadRequiredDecimal(item, "size"),
            });
        }

        return items;
    }
}
