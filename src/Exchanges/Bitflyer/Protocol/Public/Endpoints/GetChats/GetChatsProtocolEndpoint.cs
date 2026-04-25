using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetChats;

public interface IGetChatsProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? fromDate,
        CancellationToken cancellationToken = default);
}

public sealed class GetChatsProtocolEndpoint : IGetChatsProtocolEndpoint
{
    private const string Path = "/v1/getchats";
    private readonly IProtocolTransport _transport;

    public GetChatsProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? fromDate,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetChats,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.CreateSingle("from_date", fromDate),
            BodyText = null,
        };

        var result = await _transport.SendAsync(request, ProtocolTransportAuthMode.None, cancellationToken);
        return ProtocolCallFactory.ToProtocolCall(
            request,
            result,
            scope: "Public",
            auth: "None",
            component: CallComponents.PublicEndpointModule);
    }
}
