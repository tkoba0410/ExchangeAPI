using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;

public interface IGetBankAccountsProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        CancellationToken cancellationToken = default);
}

public sealed class GetBankAccountsProtocolEndpoint : IGetBankAccountsProtocolEndpoint
{
    private const string Path = "/v1/me/getbankaccounts";
    private readonly IProtocolTransport _transport;

    public GetBankAccountsProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetBankAccounts,
            Method = HttpMethods.Get,
            Path = Path,
            Query = null,
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
