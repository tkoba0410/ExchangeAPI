using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetCorporateLeverage;

public interface IGetCorporateLeverageProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        CancellationToken cancellationToken = default);
}

public sealed class GetCorporateLeverageProtocolEndpoint : IGetCorporateLeverageProtocolEndpoint
{
    private const string Path = "/v1/getcorporateleverage";
    private readonly IProtocolTransport _transport;

    public GetCorporateLeverageProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetCorporateLeverage,
            Method = HttpMethods.Get,
            Path = Path,
            Query = null,
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
