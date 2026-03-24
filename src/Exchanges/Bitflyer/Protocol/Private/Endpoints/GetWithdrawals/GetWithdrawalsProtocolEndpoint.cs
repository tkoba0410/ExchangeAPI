using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;

public interface IGetWithdrawalsProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        string? messageId,
        CancellationToken cancellationToken = default);
}

public sealed class GetWithdrawalsProtocolEndpoint : IGetWithdrawalsProtocolEndpoint
{
    private const string Path = "/v1/me/getwithdrawals";
    private readonly IProtocolTransport _transport;

    public GetWithdrawalsProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        string? messageId,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetWithdrawals,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.Create(
                ("count", count?.ToString(CultureInfo.InvariantCulture)),
                ("before", before?.ToString(CultureInfo.InvariantCulture)),
                ("after", after?.ToString(CultureInfo.InvariantCulture)),
                ("message_id", messageId)),
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
