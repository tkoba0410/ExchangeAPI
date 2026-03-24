using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetHealth;

public interface IGetHealthProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        CancellationToken cancellationToken = default);
}

public sealed class GetHealthProtocolEndpoint : IGetHealthProtocolEndpoint
{
    private const string Path = "/v1/gethealth";
    private readonly IProtocolTransport _transport;

    public GetHealthProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetHealth,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.CreateSingle("product_code", productCode),
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
