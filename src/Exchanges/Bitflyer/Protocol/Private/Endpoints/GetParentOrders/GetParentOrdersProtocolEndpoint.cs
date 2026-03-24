using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;

public interface IGetParentOrdersProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        CancellationToken cancellationToken = default);
}

public sealed class GetParentOrdersProtocolEndpoint : IGetParentOrdersProtocolEndpoint
{
    private const string Path = "/v1/me/getparentorders";
    private readonly IProtocolTransport _transport;

    public GetParentOrdersProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetParentOrders,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.Create(
                ("product_code", productCode),
                ("count", count?.ToString(CultureInfo.InvariantCulture)),
                ("before", before?.ToString(CultureInfo.InvariantCulture)),
                ("after", after?.ToString(CultureInfo.InvariantCulture)),
                ("parent_order_state", parentOrderState)),
            BodyText = null,
        };

        var result = await _transport.SendAsync(request, ProtocolTransportAuthMode.KeySecret, cancellationToken);
        return ProtocolCallFactory.ToProtocolCall(
            request,
            result,
            scope: "Private",
            auth: "KeySecret",
            component: CallComponents.PrivateEndpointModule);
    }
}
