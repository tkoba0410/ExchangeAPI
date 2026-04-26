using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;

public interface IGetChatsNativeEndpoint
{
    Task<CallResult<GetChatsRequest, IReadOnlyList<GetChats.Item>>> CallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetChatsNativeEndpoint : IGetChatsNativeEndpoint
{
    private readonly IGetChatsProtocolEndpoint _protocolEndpoint;

    public GetChatsNativeEndpoint(IGetChatsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<CallResult<GetChatsRequest, IReadOnlyList<GetChats.Item>>> CallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.FromDate is not null && string.IsNullOrWhiteSpace(request.FromDate))
        {
            return NativeCallFactory.Failure<GetChatsRequest, IReadOnlyList<GetChats.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Semantic, Message = "FromDate must not be blank." },
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetChats,
                scope: "Public",
                auth: "None");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.FromDate, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetChatsRequest, IReadOnlyList<GetChats.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetChats,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetChatsRequest, IReadOnlyList<GetChats.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetChats,
                "Public",
                "None");
        }

        try
        {
            var root = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetChats.Item>();
            foreach (var item in root.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    throw new CodecException("Chat item must be an object.");
                }

                items.Add(new GetChats.Item
                {
                    Nickname = JsonValueReader.ReadRequiredString(item, "nickname"),
                    Message = JsonValueReader.ReadRequiredString(item, "message"),
                    Date = JsonValueReader.ReadRequiredUtcTimestamp(item, "date"),
                });
            }

            return NativeCallFactory.Success(request, (IReadOnlyList<GetChats.Item>)items, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetChatsRequest, IReadOnlyList<GetChats.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetChats,
                "Public",
                "None");
        }
    }
}
