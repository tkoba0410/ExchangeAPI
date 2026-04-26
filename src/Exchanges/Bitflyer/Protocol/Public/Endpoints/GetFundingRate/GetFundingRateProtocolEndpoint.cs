using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetFundingRate;

public interface IGetFundingRateProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}

public sealed class GetFundingRateProtocolEndpoint : IGetFundingRateProtocolEndpoint
{
    private const string Path = "/v1/getfundingrate";
    private readonly IProtocolTransport _transport;

    public GetFundingRateProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetFundingRate,
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
