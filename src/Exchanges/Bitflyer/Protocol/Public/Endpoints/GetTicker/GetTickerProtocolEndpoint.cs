using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;

public interface IGetTickerProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        CancellationToken cancellationToken = default);
}

public sealed class GetTickerProtocolEndpoint : IGetTickerProtocolEndpoint
{
    private const string CanonicalPath = "/v1/getticker";
    private const string AliasPath = "/v1/ticker";
    private readonly IProtocolTransport _transport;
    private readonly string _path;

    public GetTickerProtocolEndpoint(IProtocolTransport transport, bool useAliasPath = false)
    {
        _transport = transport;
        _path = useAliasPath ? AliasPath : CanonicalPath;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetTicker,
            Method = HttpMethods.Get,
            Path = _path,
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
